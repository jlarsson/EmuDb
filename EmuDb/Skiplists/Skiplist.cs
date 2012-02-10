using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EmuDb.Utility;

namespace EmuDb.Skiplists
{
    public class Skiplist<TKey, TValue> : ISkiplist<TKey, TValue>
    {
        public Skiplist(ISkiplistArena<TKey, TValue> arena) : this(arena, Comparer<TKey>.Default, KeyStrategy.Unique)
        {
        }

        public Skiplist(ISkiplistArena<TKey, TValue> arena, KeyStrategy keyStrategy)
            : this(arena, Comparer<TKey>.Default, keyStrategy)
        {
        }

        public Skiplist(ISkiplistArena<TKey, TValue> arena, IComparer<TKey> keyComparer, KeyStrategy keyStrategy)
        {
            Random = new Random();
            Arena = arena;
            KeyComparer = keyComparer;
            KeyStrategy = keyStrategy;
        }

        public Random Random { get; protected set; }

        public ISkiplistNode<TKey, TValue> Head
        {
            get { return Arena.RootNode; }
            protected set { Arena.RootNode = value; }
        }

        public ISkiplistArena<TKey, TValue> Arena { get; set; }
        public IComparer<TKey> KeyComparer { get; protected set; }
        public KeyStrategy KeyStrategy { get; protected set; }

        #region ISkiplist<TKey,TValue> Members

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, KeyStrategy);
        }

        public bool ContainsKey(TKey key)
        {
            return Find(key).Any();
        }

        public ICollection<TKey> Keys
        {
            get { return this.Select(kv => kv.Key).ToList(); }
        }

        public bool Remove(TKey key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var kv in Find(key))
            {
                value = kv.Value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public ICollection<TValue> Values
        {
            get { return this.Select(kv => kv.Value).ToList(); }
        }

        public TValue this[TKey key]
        {
            get { return Find(key).First().Value; }
            set { Insert(key, value, KeyStrategy.Update); }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            Head = null;
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return Find(item.Key).Any(kv => Equals(item.Value, kv.Value));
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            this.ToList().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.Count(); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return
                Traverse(
                    Traverse(Head, n => n.Below).Last(),
                    n => n.Next)
                    .Skip(1)
                    .Select(n => n.Data.GetKeyValuePair()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> Find(TKey key)
        {
            var node = Head;
            while (true)
            {
                while ((node.Next != null) && (KeyComparer.Compare(node.Next.Data.Key, key) < 0))
                {
                    node = node.Next;
                }
                if (node.Below == null)
                {
                    break;
                }
                node = node.Below;
            }
            node = node.Next;

            return node == null
                       ? Enumerable.Empty<KeyValuePair<TKey, TValue>>()
                       : Traverse(node, n => n.Next).TakeWhile(n => KeyComparer.Compare(n.Data.Key, key) == 0).Select(
                           n => n.Data.GetKeyValuePair());
        }

        #endregion

        protected void Insert(TKey key, TValue value, KeyStrategy keyStrategy)
        {
            var data = new Lazy<ISkiplistKeyValuePair<TKey, TValue>>(() => Arena.CreateData(key, value));

            if (Head == null)
            {
                Head = Arena
                    .CreateNode(null)
                    .With(n => n.Next = Arena.CreateNode(data.Value));
                return;
            }

            // Search through the list, remembering predecessor node och each level
            var node = Head;

            var fixup = new List<ISkiplistNode<TKey, TValue>>();
            while (node != null)
            {
                while ((node.Next != null) && (KeyComparer.Compare(node.Next.Data.Key, key) < 0))
                {
                    node = node.Next;
                }
                fixup.Add(node);
                node = node.Below;
            }

            if (keyStrategy == KeyStrategy.Unique)
            {
                if ((from fixupNode in fixup
                     let duplicateCandidate = fixupNode.Next
                     where duplicateCandidate != null
                     where KeyComparer.Compare(duplicateCandidate.Data.Key, key) == 0
                     select duplicateCandidate).Any())
                {
                    throw new DuplicateKeyException();
                }
            }

            // We must start at highest level, since node added on level will be used as below pointer for next added node
            fixup.Reverse();

            // We flip a coin from lowest node and upwards to get where to start insertion
            var level = Enumerable.Range(0, int.MaxValue).First(i => (Random.Next() & 1) == 1);
            if (level >= fixup.Count)
            {
                Head = Arena.CreateNode(null)
                    .With(n => n.Below = Head);
                fixup.Add(Head);
            }


            // Go through all the predessors, linking in a new node after, setting the new node's below to previous added node
            fixup.Take(level + 1).Aggregate(
                default(ISkiplistNode<TKey, TValue>),
                (below, fixupNode) =>
                    {
                        if (keyStrategy == KeyStrategy.Update)
                        {
                            if ((fixupNode.Next != null) && (KeyComparer.Compare(fixupNode.Next.Data.Key, key) == 0))
                            {
                                fixupNode.Next.Data = data.Value;
                                return fixupNode.Next;
                            }
                        }

                        return fixupNode.Next =
                               Arena.CreateNode(data.Value).With(n => n.Next = fixupNode.Next).With(
                                   n => n.Below = below);
                    });
        }

        protected IEnumerable<T> Traverse<T>(T element, Func<T, T> getNextElement) where T : class
        {
            while (element != null)
            {
                yield return element;
                element = getNextElement(element);
            }
        }

        public void Dump(TextWriter writer)
        {
            Console.Out.WriteLine("** Skiplist **");
            var head = Head;
            while (head != null)
            {
                Console.Out.WriteLine(string.Join(",",
                                                  from node in Traverse(head, n => n.Next)
                                                  where node.Data != null
                                                  select node.Data == null ? "(null)" : (object) node.Data.Key));

                head = head.Below;
            }
        }
    }
}
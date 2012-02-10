using System.Collections.Generic;

namespace EmuDb.Skiplists
{
    public class SkiplistMemoryArena<TKey, TValue> : ISkiplistArena<TKey, TValue>
    {
        #region ISkiplistArena<TKey,TValue> Members

        public ISkiplistNode<TKey, TValue> RootNode { get; set; }

        public ISkiplistKeyValuePair<TKey, TValue> CreateData(TKey key, TValue value)
        {
            return new Record {Key = key, Value = value};
        }

        public ISkiplistNode<TKey, TValue> CreateNode(ISkiplistKeyValuePair<TKey, TValue> data)
        {
            return new Node {Data = data};
        }

        #endregion

        #region Nested type: Node

        public class Node : ISkiplistNode<TKey, TValue>
        {
            #region ISkiplistNode<TKey,TValue> Members

            public ISkiplistNode<TKey, TValue> Next { get; set; }
            public ISkiplistNode<TKey, TValue> Below { get; set; }
            public ISkiplistKeyValuePair<TKey, TValue> Data { get; set; }

            #endregion
        }

        #endregion

        #region Nested type: Record

        public class Record : ISkiplistKeyValuePair<TKey, TValue>
        {
            #region ISkiplistKeyValuePair<TKey,TValue> Members

            public TKey Key { get; set; }
            public TValue Value { get; set; }

            public KeyValuePair<TKey, TValue> GetKeyValuePair()
            {
                return new KeyValuePair<TKey, TValue>(Key, Value);
            }

            #endregion
        }

        #endregion
    }
}
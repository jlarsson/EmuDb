using System.Collections.Generic;

namespace EmuDb.Skiplists
{
    public interface ISkiplist<TKey, TValue>: IDictionary<TKey, TValue>
    {
        IEnumerable<KeyValuePair<TKey, TValue>> Find(TKey key);
    }
}

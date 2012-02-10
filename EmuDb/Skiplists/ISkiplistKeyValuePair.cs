using System.Collections.Generic;

namespace EmuDb.Skiplists
{
    public interface ISkiplistKeyValuePair<TKey,TValue>
    {
        TKey Key { get; }
        TValue Value { get; }
        KeyValuePair<TKey, TValue> GetKeyValuePair();
    }
}
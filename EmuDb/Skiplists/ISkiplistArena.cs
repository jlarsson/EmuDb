namespace EmuDb.Skiplists
{
    public interface ISkiplistArena<TKey,TValue>
    {
        ISkiplistNode<TKey, TValue> RootNode { get; set; }
        ISkiplistKeyValuePair<TKey, TValue> CreateData(TKey key, TValue value);
        ISkiplistNode<TKey, TValue> CreateNode(ISkiplistKeyValuePair<TKey, TValue> data);
    }
}
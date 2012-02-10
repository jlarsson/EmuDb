namespace EmuDb.Skiplists
{
    public interface ISkiplistNode<TKey,TValue>
    {
        ISkiplistNode<TKey, TValue> Next { get; set; }
        ISkiplistNode<TKey, TValue> Below { get; set; }
        ISkiplistKeyValuePair<TKey, TValue> Data { get; set; }
    }
}
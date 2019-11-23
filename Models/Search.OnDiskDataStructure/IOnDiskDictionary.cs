using System.Collections.Generic;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An interface for on-disk dictionary
    /// </summary>
    /// <typeparam name="TKey">Generic type of key</typeparam>
    /// <typeparam name="TValue">Generic type of value</typeparam>
    public interface IOnDiskDictionary<TKey, TValue>
    {
        TValue Get(TKey key);
        void Add(TKey key, TValue value);
        TKey[] GetKeys();
        TValue[] GetValues();
        void Clear();
        int GetSize();
        bool ContainsKey(TKey key);
        void Replace(Dictionary<TKey,TValue> dictionary);
    }
}
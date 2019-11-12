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
        void Save(SortedDictionary<TKey, TValue> valuePairs);
        TValue Get(TKey key);
        List<TValue> Get(List<TKey> key);
        TKey[] GetKeys();
        // TValue[] GetValues();
    }
}
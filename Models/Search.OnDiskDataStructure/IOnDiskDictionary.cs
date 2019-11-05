using System.Collections.Generic;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implmentation of on-disk dictionary
    /// </summary>
    /// <typeparam name="TKey">Generic type of key</typeparam>
    /// <typeparam name="TValue">Generic type of value</typeparam>
    public interface IOnDiskDictionary<TKey, TValue>
    {
        void Save(IEncoderDecoder<TKey> keyEncoderDecoder, IEncoderDecoder<TValue> valueEncoderDecoder, Dictionary<TKey, TValue> valuePairs, string path, string fileName);
        TValue Get(IEncoderDecoder<TKey> keyEncoderDecoder, IEncoderDecoder<TValue> valueEncoderDecoder, TKey key, string path, string fileName);
        TKey[] GetKeys(IEncoderDecoder<TKey> keyEncoderDecoder, string path, string fileName);
        TValue[] GetValues(IEncoderDecoder<TValue> valueEncoderDecoder, string path, string fileName);
    }
}
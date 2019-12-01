using CSharpTest.Net.Collections;
using CSharpTest.Net.Serialization;
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
namespace Search.OnDiskDataStructure
{
    public class OnDiskDictionary<TKey, TValue> : IOnDiskDictionary<TKey, TValue> where TKey : IComparable
    {

        private IEncoderDecoder<TKey> keyED;
        private IEncoderDecoder<TValue> valueED;
        private BPlusTree<TKey, byte[]> map;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dictName"></param>
        /// <param name="keyEncoderDecoder"></param>
        /// <param name="valueEncoderDecoder"></param>
        public OnDiskDictionary(string path, string dictName, IEncoderDecoder<TKey> keyEncoderDecoder, IEncoderDecoder<TValue> valueEncoderDecoder)
        {
            this.keyED = keyEncoderDecoder;
            this.valueED = valueEncoderDecoder;
            ISerializer<TKey> tKeySerializer = null;
            if (typeof(TKey) == typeof(int))
            {
                tKeySerializer = (ISerializer<TKey>)(PrimitiveSerializer.Int32);

            }
            else
            {
                tKeySerializer = (ISerializer<TKey>)(PrimitiveSerializer.String);

            }

            BPlusTree<TKey, byte[]>.Options options = new BPlusTree<TKey, byte[]>.Options(tKeySerializer, PrimitiveSerializer.Bytes)
            {
                CreateFile = CreatePolicy.IfNeeded,
                FileName = Path.Join(path, dictName + ".bin")
            };

            map = new BPlusTree<TKey, byte[]>(options);
        }

        ///Release map on distruction of this object
        ~OnDiskDictionary()
        {
            map.Dispose();
        }

        /// <summary>
        /// Get value for a given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            TValue result;
            if (map.ContainsKey(key))
            {
                result = valueED.Decoding(map[key]);
            }
            else
            {
                result = default(TValue);
            }
            return result;
        }

        /// <summary>
        /// Add pair if key doesn't exist. Else replace value for key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            if (map.ContainsKey(key))
            {
                map[key] = valueED.Encoding(value);
                
            }
            else
            {
                map.Add(key, valueED.Encoding(value));
            }
        }

        /// <summary>
        /// Clear dictionary
        /// </summary>
        public void Clear()
        {
            map.Clear();
        }

        /// <summary>
        /// Get all kyes
        /// </summary>
        /// <returns></returns>
        public TKey[] GetKeys()
        {
            List<TKey> result = new List<TKey>();
            foreach (TKey key in map.Keys)
            {
                result.Add(key);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns></returns>
        public TValue[] GetValues()
        {
            List<TValue> values = new List<TValue>();
            foreach (byte[] rawValue in map.Values)
            {
                values.Add(valueED.Decoding(rawValue));
            }
            return values.ToArray();
        }

        /// <summary>
        /// Get size of dictionary
        /// </summary>
        /// <returns></returns>
        public int GetSize()
        {
            return this.GetKeys().Length;
        }

        /// <summary>
        /// Check if dictionary contains a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            return map.ContainsKey(key);
        }

        /// <summary>
        /// Replace on-disk dictionary with a given dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public void Replace(Dictionary<TKey, TValue> dictionary)
        {
            map.Clear();
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                this.Add(pair.Key, pair.Value);
            }
        }

    }
}
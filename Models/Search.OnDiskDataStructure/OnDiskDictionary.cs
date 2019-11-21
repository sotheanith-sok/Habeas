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

        ~OnDiskDictionary()
        {
            map.Dispose();
        }

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

        public void Clear()
        {
            map.Clear();
        }

        public TKey[] GetKeys()
        {
            List<TKey> result = new List<TKey>();
            foreach (TKey key in map.Keys)
            {
                result.Add(key);
            }
            return result.ToArray();
        }

        public TValue[] GetValues()
        {
            List<TValue> values = new List<TValue>();
            foreach (byte[] rawValue in map.Values)
            {
                values.Add(valueED.Decoding(rawValue));
            }
            return values.ToArray();
        }

        public int GetSize()
        {
            return this.GetKeys().Length;
        }

        public bool ContainsKey(TKey key)
        {
            return map.ContainsKey(key);
        }

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
using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using LiteDB;
namespace Search.OnDiskDataStructure
{
    public class OnDiskDictionary<TKey, TValue> : IOnDiskDictionary<TKey, TValue> where TKey : IComparable
    {

        private IEncoderDecoder<TKey> keyED;
        private IEncoderDecoder<TValue> valueED;
        private LiteDatabase database;
        private LiteCollection<DBObject<TKey>> collection;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dictName"></param>
        /// <param name="keyEncoderDecoder"></param>
        /// <param name="valueEncoderDecoder"></param>
        public OnDiskDictionary(string path, string dictName, IEncoderDecoder<TKey> keyEncoderDecoder, IEncoderDecoder<TValue> valueEncoderDecoder)
        {
            System.IO.Directory.CreateDirectory(path);
            this.keyED = keyEncoderDecoder;
            this.valueED = valueEncoderDecoder;
            this.database = new LiteDatabase(Path.Join(path, dictName + ".bin"));
            this.collection = this.database.GetCollection<DBObject<TKey>>(dictName);
        }

        ///Release database on distruction of this object
        ~OnDiskDictionary()
        {
            this.database.Dispose();
        }

        /// <summary>
        /// Get value for a given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue Get(TKey key)
        {
            TValue result = default(TValue);
            foreach (DBObject<TKey> obj in this.collection.Find(x => x.key.ToString().Equals(key.ToString())))
            {
                result = valueED.Decoding(obj.raw_value);
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
            if (this.collection.Exists(x => x.key.ToString().Equals(key.ToString())))
            {

            }
            foreach (DBObject<TKey> obj in this.collection.Find(x => x.key.ToString().Equals(key.ToString())))
            {
                obj.raw_value = valueED.Encoding(value);
                this.collection.Update(obj);
                return;
            }

            this.collection.Insert(new DBObject<TKey>
            {
                key = key,
                raw_value = valueED.Encoding(value)
            });
        }

        /// <summary>
        /// Clear dictionary
        /// </summary>
        public void Clear()
        {
            this.collection.Delete(x => true);
        }

        /// <summary>
        /// Get all kyes
        /// </summary>
        /// <returns></returns>
        public TKey[] GetKeys()
        {
            List<TKey> keys = new List<TKey>();
            foreach (DBObject<TKey> obj in this.collection.Find(x => true))
            {
                keys.Add(obj.key);
            }
            return keys.ToArray();
        }

        /// <summary>
        /// Get all values
        /// </summary>
        /// <returns></returns>
        public TValue[] GetValues()
        {
            List<TValue> values = new List<TValue>();
            foreach (DBObject<TKey> obj in this.collection.Find(x => true))
            {
                values.Add(valueED.Decoding(obj.raw_value));
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
            return this.collection.Exists(x => x.key.ToString().Equals(key.ToString()));
        }

        /// <summary>
        /// Replace on-disk dictionary with a given dictionary
        /// </summary>
        /// <param name="dictionary"></param>
        public void Replace(Dictionary<TKey, TValue> dictionary)
        {
            this.Clear();
            List<DBObject<TKey>> temp = new List<DBObject<TKey>>();
            foreach (KeyValuePair<TKey, TValue> pair in dictionary)
            {
                temp.Add(new DBObject<TKey>
                {
                    key = pair.Key,
                    raw_value = valueED.Encoding(pair.Value)
                });
            }

            this.collection.InsertBulk(temp);

        }

    }

    public class DBObject<T>
    {
        public int id { get; set; }
        public T key { get; set; }
        public byte[] raw_value { get; set; }
    }
}
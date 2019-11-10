using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of on-disk dictionary
    /// </summary>
    /// <typeparam name="TKey">Type of Key</typeparam>
    /// <typeparam name="TValue">Type of Value</typeparam>
    public class OnDiskDictionary<TKey, TValue> : IOnDiskDictionary<TKey, TValue> where TKey : IComparable
    {
        private IEncoderDecoder<TKey> keyEncoderDecoder;
        private IEncoderDecoder<TValue> valueEncoderDecoder;

        /// <summary>
        /// Construct a new on-disk dictionary
        /// </summary>
        /// <param name="keyEncoderDecoder">Encoder/Decoder for key</param>
        /// <param name="valueEncoderDecoder">Encoder/Decoder for value</param>
        public OnDiskDictionary(IEncoderDecoder<TKey> keyEncoderDecoder, IEncoderDecoder<TValue> valueEncoderDecoder)
        {
            this.keyEncoderDecoder = keyEncoderDecoder;
            this.valueEncoderDecoder = valueEncoderDecoder;
        }

        /// <summary>
        /// Save a dictionary onto disk
        /// </summary>
        /// <param name="valuePairs">Dictionary</param>
        /// <param name="path">Path to save disk to</param>
        /// <param name="fileName">Filename of dictionary</param>
        public void Save(SortedDictionary<TKey, TValue> valuePairs, string path, string fileName)
        {
            path = Path.GetFullPath(path);
            string keyBin = Path.Join(path, fileName + "_Key.bin");
            string valueBin = Path.Join(path, fileName + "_Value.bin");
            string tableBin = Path.Join(path, fileName + "_Table.bin");

            long[] keyPositions = this.WriteKeyBin(valuePairs.Keys.ToList(), keyBin);
            long[] valuePositions = this.WriteValueBin(valuePairs.Values.ToList(), valueBin);
            this.WriteTableBin(keyPositions, valuePositions, tableBin);

        }


        /// <summary>
        /// Get value for a given key
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="path">Path to save disk to</param>
        /// <param name="fileName">Filename of dictionary</param>
        /// <returns>Value related to a given key</returns>
        public TValue Get(TKey key, string path, string fileName)
        {
            path = Path.GetFullPath(path);
            string keyBin = Path.Join(path, fileName + "_Key.bin");
            string valueBin = Path.Join(path, fileName + "_Value.bin");
            string tableBin = Path.Join(path, fileName + "_Table.bin");

            long[] table = this.ReadTableBin(tableBin);
            int index = this.ReadKeyBin(table, keyBin, key);
            TValue value = this.ReadValueBin(table, index, valueBin);

            return value;
        }


        /// <summary>
        /// Write keys to bin
        /// </summary>
        /// <param name="key">List of keys</param>
        /// <param name="path">Path to save keys to</param>
        /// <returns>array of positions</returns>
        private long[] WriteKeyBin(List<TKey> keys, string path)
        {
            List<long> keyPositions = new List<long>();
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                foreach (TKey key in keys)
                {
                    keyPositions.Add(writer.BaseStream.Position);
                    writer.Write(keyEncoderDecoder.Encoding(key));
                }
            }
            return keyPositions.ToArray();
        }

        /// <summary>
        /// Write values to bin
        /// </summary>
        /// <param name="values">List of values</param>
        /// <param name="path">Path to save values to</param>
        /// <returns>array of positions</returns>
        private long[] WriteValueBin(List<TValue> values, string path)
        {
            List<long> valuePositions = new List<long>();
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                foreach (TValue value in values)
                {
                    valuePositions.Add(writer.BaseStream.Position);
                    writer.Write(valueEncoderDecoder.Encoding(value));
                }
            }
            return valuePositions.ToArray();
        }

        /// <summary>
        /// Write positions to table bin
        /// </summary>
        /// <param name="keyPositions">Array of key positions</param>
        /// <param name="valuePositions">Array of value positions</param>
        /// <param name="path">Path to save table to</param>
        private void WriteTableBin(long[] keyPositions, long[] valuePositions, string path)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                for (int i = 0; i < keyPositions.Length; i++)
                {
                    writer.Write(keyPositions[i]);
                    writer.Write(valuePositions[i]);
                }
            }
        }


        /// <summary>
        /// Read table from bin files
        /// </summary>
        /// <param name="path">Path to table bin</param>
        /// <returns>Array of positions</returns>
        private long[] ReadTableBin(string path)
        {
            List<long> tableBin = new List<long>();
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    tableBin.Add(reader.ReadInt64());
                    tableBin.Add(reader.ReadInt64());
                }
            }
            return tableBin.ToArray();
        }

        /// <summary>
        /// Search for key in key bin
        /// </summary>
        /// <param name="table">Array of positions</param>
        /// <param name="path">Path to key bin</param>
        /// <param name="term">term to search for</param>
        /// <returns>index in table where key is located</returns>
        private int ReadKeyBin(long[] table, string path, TKey term)
        {
            using (FileStream file = File.Open(path, FileMode.Open))
            using (BinaryReader binaryReader = new BinaryReader(file))
            {
                int i = 0;
                int j = table.Length / 2 - 1;
                while (i <= j)
                {
                    int m = (i + j) / 2;
                    long termStartByte = table[m * 2];

                    long length = m * 2 < table.Length - 2 ? table[m * 2 + 2] - table[m * 2] : file.Length - table[m * 2];
                    binaryReader.BaseStream.Position = termStartByte;
                    TKey termFromFile = keyEncoderDecoder.Decoding((binaryReader.ReadBytes((int)(length))));

                    int compareValue = term.CompareTo(termFromFile);
                    if (compareValue == 0)
                    {
                        // found it!
                        return m * 2 + 1;
                    }
                    else if (compareValue < 0)
                    {
                        j = m - 1;
                    }
                    else
                    {
                        i = m + 1;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Read value from value bin
        /// </summary>
        /// <param name="table">Array of positions</param>
        /// <param name="index">Index where key was found</param>
        /// <param name="path">Path to value bin</param>
        /// <returns>Value</returns>
        private TValue ReadValueBin(long[] table, int index, string path)
        {
            if (index == -1)
            {
                Console.WriteLine(default(TValue));
                return default(TValue);
            }
            else
            {
                using (FileStream file = File.Open(path, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(file))
                {
                    long startByte = table[index];
                    long length = index < table.Length - 2 ? table[index + 2] - table[index] : file.Length - table[index];
                    reader.BaseStream.Position = startByte;
                    TValue result = valueEncoderDecoder.Decoding(reader.ReadBytes((int)(length)));
                    return result;
                }
            }
        }

        /// <summary>
        /// Get all keys
        /// </summary>
        /// <param name="path">Path to keys bin</param>
        /// <param name="fileName">Name of keys bin</param>
        /// <returns>Array of Keys</returns>
        public TKey[] GetKeys(string path, string fileName)
        {
            path = Path.GetFullPath(path);
            string keyBin = Path.Join(path, fileName + "_Key.bin");
            string valueBin = Path.Join(path, fileName + "_Value.bin");
            string tableBin = Path.Join(path, fileName + "_Table.bin");

            long[] table = this.ReadTableBin(tableBin);
            List<TKey> keys = new List<TKey>();

            using (FileStream file = File.Open(keyBin, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(file))
            {
                long previous = -1;
                long current = -1;
                for (int i = 0; i < table.Length; i += 2)
                {
                    if (current == -1)
                    {
                        current = i;
                    }
                    else
                    {
                        previous = current;
                        current = i;
                        reader.BaseStream.Position = table[previous];
                        keys.Add(keyEncoderDecoder.Decoding(reader.ReadBytes((int)(table[current] - table[previous]))));
                    }
                }
                reader.BaseStream.Position = table[current];
                keys.Add(keyEncoderDecoder.Decoding(reader.ReadBytes((int)(reader.BaseStream.Length - table[current]))));

            }
            return keys.ToArray();
        }

        /// <summary>
        /// Get all values from values bin
        /// </summary>
        /// <param name="path">Path to values bin</param>
        /// <param name="fileName">Name of values bin</param>
        /// <returns></returns>
        public TValue[] GetValues(string path, string fileName)
        {
            path = Path.GetFullPath(path);
            string keyBin = Path.Join(path, fileName + "_Key.bin");
            string valueBin = Path.Join(path, fileName + "_Value.bin");
            string tableBin = Path.Join(path, fileName + "_Table.bin");

            long[] table = this.ReadTableBin(tableBin);
            List<TValue> keys = new List<TValue>();

            using (FileStream file = File.Open(valueBin, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(file))
            {
                long previous = -1;
                long current = -1;
                for (int i = 1; i < table.Length; i += 2)
                {

                    if (current == -1)
                    {
                        current = i;
                    }
                    else
                    {
                        previous = current;
                        current = i;
                        reader.BaseStream.Position = table[previous];
                        keys.Add(valueEncoderDecoder.Decoding(reader.ReadBytes((int)(table[current] - table[previous]))));
                    }
                }
                reader.BaseStream.Position = table[current];
                keys.Add(valueEncoderDecoder.Decoding(reader.ReadBytes((int)(reader.BaseStream.Length - table[current]))));
            }
            return keys.ToArray();
        }


        /// <summary>
        /// Search for key bin for a list of term positions
        /// </summary>
        /// <param name="table">Array of positions</param>
        /// <param name="path">Path to key bin</param>
        /// <param name="term">term to search for</param>
        /// <returns>index in table where key is located</returns>
        private List<int> ReadKeyBin(long[] table, string path, List<TKey> terms)
        {
            List<int> result = new List<int>();
            using (FileStream file = File.Open(path, FileMode.Open))
            using (BinaryReader binaryReader = new BinaryReader(file))
            {
                foreach (TKey term in terms)
                {
                    int i = 0;
                    int j = table.Length / 2 - 1;
                    while (i <= j)
                    {
                        int m = (i + j) / 2;
                        long termStartByte = table[m * 2];

                        long length = m * 2 < table.Length - 2 ? table[m * 2 + 2] - table[m * 2] : file.Length - table[m * 2];
                        binaryReader.BaseStream.Position = termStartByte;
                        TKey termFromFile = keyEncoderDecoder.Decoding((binaryReader.ReadBytes((int)(length))));

                        int compareValue = term.CompareTo(termFromFile);
                        if (compareValue == 0)
                        {
                            // found it!
                            result.Add(m * 2 + 1);
                            break;
                        }
                        else if (compareValue < 0)
                        {
                            j = m - 1;
                        }
                        else
                        {
                            i = m + 1;
                        }
                    }
                    result.Add(-1);
                }

            }
            return result;

        }


        /// <summary>
        /// Get all values for a list of keys
        /// </summary>
        /// <param name="path">Path to values bin</param>
        /// <param name="fileName">Name of values bin</param>
        /// <returns></returns>
        private List<TValue> ReadValueBin(long[] table, List<int> indexes, string path)
        {
            List<TValue> results = new List<TValue>();

            using (FileStream file = File.Open(path, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(file))
            {
                foreach (int index in indexes)
                {
                    if (index == -1)
                    {
                        results.Add(default(TValue));
                    }
                    else
                    {
                        long startByte = table[index];
                        long length = index < table.Length - 2 ? table[index + 2] - table[index] : file.Length - table[index];
                        reader.BaseStream.Position = startByte;
                        TValue result = valueEncoderDecoder.Decoding(reader.ReadBytes((int)(length)));
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        /// <summary>
        /// Get List of value for a list of key
        /// </summary>
        /// <param name="key">Key to search for</param>
        /// <param name="path">Path to save disk to</param>
        /// <param name="fileName">Filename of dictionary</param>
        /// <returns>Value related to a given key</returns>
        public List<TValue> Get(List<TKey> key, string path, string fileName)
        {
            path = Path.GetFullPath(path);
            string keyBin = Path.Join(path, fileName + "_Key.bin");
            string valueBin = Path.Join(path, fileName + "_Value.bin");
            string tableBin = Path.Join(path, fileName + "_Table.bin");

            long[] table = this.ReadTableBin(tableBin);
            List<int> indexes = this.ReadKeyBin(table, keyBin, key);
            List<TValue> values = this.ReadValueBin(table, indexes, valueBin);

            return values;
        }
    }


}



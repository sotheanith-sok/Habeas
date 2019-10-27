using System.Collections.Generic;
using System.IO;
using Search.Document;
using System.Text;
using System;
using System.Linq;
namespace Search.Index
{
    /// <summary>
    /// Class reponsible to reading KGram from disk
    /// </summary>
    public class DiskKGramReader
    {
        /// <summary>
        /// Get possible candidates for a given KGram
        /// </summary>
        /// <param name="kGram">given kGram</param>
        /// <param name="path">Path to where the KGram files located</param>
        /// <returns></returns>
        public List<string> GetCandidates(string kGram, string path)
        {
            path = Path.GetFullPath(path);
            List<Entry> entries = GenerateEntries(path, "kGramTable.bin");
            return SearchValuesBin(SearchKeysBin(entries, kGram, path, "kGram.bin"), path, "candidates.bin");
        }

        /// <summary>
        /// Get possible KGrams from a given mini KGram
        /// </summary>
        /// <param name="kGram">mini KGram</param>
        /// <param name="path">Path to where the KGram files located</param>
        /// <returns></returns>
        public List<string> GetPossibleKGram(string kGram, string path)
        {

            path = Path.GetFullPath(path);
            List<Entry> entries = GenerateEntries(path, "miniKGramTable.bin");
            return SearchValuesBin(SearchKeysBin(entries, kGram, path, "miniKGram.bin"), path, "miniCandidates.bin");
        }

        /// <summary>
        /// Search through entires of table bin for a given entry
        /// </summary>
        /// <param name="entries">Internal representation of table bin</param>
        /// <param name="kGram">KGram to be search for</param>
        /// <param name="path">Path to where the KGram files located</param>
        /// <param name="filename">name keys bin tie to values in table bin</param>
        /// <returns>An entry for a given KGram</returns>
        private Entry SearchKeysBin(List<Entry> entries, string kGram, string path, string filename)
        {
            string kGramBin = Path.Join(path, filename);

            int midPoint = entries.Count / 2;
            string key = "";
            Entry entry = null;
            using (BinaryReader binaryReader = new BinaryReader(File.Open(kGramBin, FileMode.Open)))
            {
                midPoint = entries.Count / 2;
                entry = entries[midPoint];
                binaryReader.BaseStream.Position = entry.keyStart;
                key = Encoding.UTF8.GetString(entry.keyLength != -1 ? binaryReader.ReadBytes(entry.keyLength) : binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position)));
            }

            Console.WriteLine(key);
            if (string.Compare(key, kGram) == 0)
            {
                return entry;
            }

            if (entries.Count > 1)
            {
                if (midPoint > 0 && string.Compare(key, kGram) == 1)
                {
                    return SearchKeysBin(entries.ToList().GetRange(0, midPoint), kGram, path, filename);
                }
                else if (midPoint < entries.Count - 1 && string.Compare(key, kGram) == -1)
                {
                    return SearchKeysBin(entries.ToList().GetRange(midPoint + 1, entries.Count - midPoint - 1), kGram, path, filename);
                }

            }
            return null;
        }


        /// <summary>
        /// Search values bin for a given entry
        /// </summary>
        /// <param name="entry">Entry to be search fro</param>
        /// <param name="path">Path to where the KGram files located</param>
        /// <param name="filename">name of values bin tie to table bin</param>
        /// <returns></returns>
        private List<string> SearchValuesBin(Entry entry, string path, string filename)
        {
            if (entry == null)
            {
                return new List<string>();
            }
            else
            {
                string candidatesBin = Path.Join(path, filename);

                using (BinaryReader reader = new BinaryReader(File.Open(candidatesBin, FileMode.Open)))
                {
                    reader.BaseStream.Position = entry.valueStart;
                    string result = Encoding.UTF8.GetString(reader.ReadBytes(entry.valueLength));
                    return result.Split(" ").ToList();
                }
            }
        }



        /// <summary>
        /// Generate entries based on a given table bin
        /// </summary>
        /// <param name="path">Path to where the KGram files located</param>
        /// <param name="filename">Name of table bin</param>
        /// <returns></returns>
        private List<Entry> GenerateEntries(string path, string filename)
        {
            string tableBin = Path.Join(path, filename);
            List<Entry> entries = new List<Entry>();
            long previousKey = -1, previousValue = -1, currentKey = -1, currentValue = -1;
            using (BinaryReader binaryReader = new BinaryReader(File.Open(tableBin, FileMode.Open)))
            {
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                {
                    if (currentKey != -1)
                    {
                        previousKey = currentKey;
                        previousValue = currentValue;
                    }

                    currentKey = binaryReader.ReadInt64();
                    currentValue = binaryReader.ReadInt64();

                    if (previousKey != -1)
                    {
                        Entry entry = new Entry();
                        entry.keyStart = previousKey;
                        entry.keyLength = (int)(currentKey - previousKey);
                        entry.valueStart = previousValue;
                        entry.valueLength = (int)(currentValue - previousValue);
                        entries.Add(entry);
                    }
                }

                //Add the final entry
                Entry finalEntry = new Entry();
                finalEntry.keyStart = currentKey;
                finalEntry.keyLength = -1;
                finalEntry.valueStart = currentValue;
                finalEntry.valueLength = -1;
                entries.Add(finalEntry);

                return entries;
            }
        }

        /// <summary>
        /// A representation of bytes entry in table bin.
        /// </summary>
        private class Entry
        {
            /// <summary>
            /// Key start byte location
            /// </summary>
            /// <value></value>
            public long keyStart { get; set; }

            /// <summary>
            /// Key bytes length
            /// </summary>
            /// <value></value>
            public int keyLength { get; set; }

            /// <summary>
            /// Value start byte location
            /// </summary>
            /// <value></value>
            public long valueStart { get; set; }

            /// <summary>
            /// Value length byte
            /// </summary>
            /// <value></value>
            public int valueLength { get; set; }
        }
    }
}
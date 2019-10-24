using System.Collections.Generic;
using System.IO;
using Search.Document;
using System.Text;
using System;
using System.Linq;
namespace Search.Index
{
    public class DiskKGramReader
    {
        public List<string> GetCandidates(string kGram, string path)
        {
            path = Path.GetFullPath(path);
            List<Entry> entries = GenerateEntries(path, "kGramTable.bin");
            return SearchValuesBin(SearchKeysBin(entries, kGram, path, "kGram.bin"), path, "candidates.bin");
        }

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
                    return SearchKeysBin(entries.GetRange(0, midPoint), kGram, path, filename);
                }
                else if (midPoint < entries.Count - 1 && string.Compare(key, kGram) == -1)
                {
                    return SearchKeysBin(entries.GetRange(midPoint + 1, entries.Count - midPoint - 1), kGram, path, filename);
                }

            }
            return null;
        }


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

        public List<string> GetPossibleKGram(string kGram, string path)
        {
            path = Path.GetFullPath(path);
            List<Entry> entries = GenerateEntries(path, "miniKGramTable.bin");
            return SearchValuesBin(SearchKeysBin(entries, kGram, path, "miniKGram.bin"), path, "miniCandidates.bin");
        }


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
        private class Entry
        {
            public long keyStart { get; set; }
            public int keyLength { get; set; }

            public long valueStart { get; set; }
            public int valueLength { get; set; }
            public override string ToString()
            {
                return keyStart.ToString("X");
            }

        }
    }
}
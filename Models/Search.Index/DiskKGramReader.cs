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
            long[] table = GenerateTable(path, "kGramTable.bin");
            List<string> result = SearchValuesBin(table, SearchKeysBin(kGram, table, path, "kGram.bin"), path, "candidates.bin");
            return result;
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
            long[] table = GenerateTable(path, "miniKGramTable.bin");
            List<string> result = SearchValuesBin(table, SearchKeysBin(kGram, table, path, "miniKGram.bin"), path, "miniCandidates.bin");
            return result;
        }



        /// <summary>
        /// Search values bin for a given entry
        /// </summary>
        /// <param name="entry">Entry to be search fro</param>
        /// <param name="path">Path to where the KGram files located</param>
        /// <param name="filename">name of values bin tie to table bin</param>
        /// <returns>List of string for result</returns>
        private List<string> SearchValuesBin(long[] table, int index, string path, string filename)
        {
            if (index == -1)
            {
                return new List<string>();
            }
            else
            {
                string candidatesBin = Path.Join(path, filename);


                using (FileStream file = File.Open(candidatesBin, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(file))
                {
                    long startByte = table[index];
                    long length = index < table.Length - 2 ? table[index + 2] - table[index] : file.Length - table[index];
                    reader.BaseStream.Position = startByte;
                    string result = Encoding.UTF8.GetString(reader.ReadBytes((int)(length)));
                    return result.Split(" ").ToList();
                }
            }
        }



        /// <summary>
        /// Generate table based on a given table bin
        /// </summary>
        /// <param name="path">Path to where the KGram files located</param>
        /// <param name="filename">Name of table bin</param>
        /// <returns>array of long</returns>
        private long[] GenerateTable(string path, string filename)
        {
            string tableBin = Path.Join(path, filename);
            List<long> keyTable = new List<long>();
            try
            {
                using (FileStream file = File.Open(tableBin, FileMode.Open))
                using (BinaryReader binaryReader = new BinaryReader(file))
                {
                    while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                    {
                        keyTable.Add(binaryReader.ReadInt64());
                        keyTable.Add(binaryReader.ReadInt64());
                    }
                }
                return keyTable.ToArray();

            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }

        /// <summary>
        ///Performance binary search on the key bin
        /// </summary>
        /// <param name="term">the term to find</param>
        /// <returns>the byte position of the postings</returns>
        private int SearchKeysBin(string term, long[] table, string path, string filename)
        {
            string keyBinPath = Path.Join(path, filename);

            try
            {
                using (FileStream file = File.Open(keyBinPath, FileMode.Open))
                using (BinaryReader binaryReader = new BinaryReader(file))
                {
                    int i = 0;
                    int j = table.Length / 2 - 1;
                    while (i <= j)
                    {
                        int m = (i + j) / 2;
                        long termStartByte = table[m * 2];

                        long length = m*2 < table.Length - 2 ? table[m * 2 + 2] - table[m * 2] : file.Length - table[m * 2];
                        binaryReader.BaseStream.Position=termStartByte;
                        string termFromFile =Encoding.UTF8.GetString(binaryReader.ReadBytes((int)(length)));

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

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return -1;
        }

    }


}
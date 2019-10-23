using System.Collections.Generic;
using System.IO;
using Search.Document;
using System.Text;
using System;
namespace Search.Index
{
    public class DiskKGramReader
    {
        public List<string> getCandidates(string kGram, string path)
        {
            path = Path.GetFullPath(path);
            string kGramBin = Path.Join(path, "kGram.bin");
            string kGramTableBin = Path.Join(path, "kGramTable.bin");
            string candidatesBin = Path.Join(path, "candidates.bin");

            using (BinaryReader candidatesBinReader = new BinaryReader(File.Open(candidatesBin, FileMode.Open)))
            using (BinaryReader kGramBinReader = new BinaryReader(File.Open(kGramBin, FileMode.Open)))
            using (BinaryReader kGramTableBinReader = new BinaryReader(File.Open(kGramTableBin, FileMode.Open)))
            {
                long previousKey = 0;
                long previousValue = 0;
                long currentKey = -1;
                long currentValue = -1;

                while (kGramTableBinReader.BaseStream.Position != kGramTableBinReader.BaseStream.Length)
                {
                    Console.WriteLine("Position: " + kGramTableBinReader.BaseStream.Position);
                    Console.WriteLine("Length: " + kGramTableBinReader.BaseStream.Length);
                    if (currentKey == -1)
                    {
                        currentKey = kGramTableBinReader.ReadInt64();
                        currentValue = kGramTableBinReader.ReadInt64();
                    }
                    else
                    {
                        previousKey = currentKey;
                        previousValue = currentValue;
                        currentKey = kGramTableBinReader.ReadInt64();
                        currentValue = kGramTableBinReader.ReadInt64();
                    }
                    Console.WriteLine(previousKey + ":" + currentKey);
                }

            }

            return new List<string>();
        }

        public List<string> getPossibleKGram(string kGram, string path)
        {
            path = Path.GetFullPath(path);
            string minKGramBin = Path.Join(path, "miniKGram.bin");
            string miniKGramTableBin = Path.Join(path, "miniKGramTable.bin");
            string miniCandidatesBin = Path.Join(path, "miniCandidates.bin");



            return new List<string>();
        }
    }
}
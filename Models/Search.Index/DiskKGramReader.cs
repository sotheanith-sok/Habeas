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
        public List<string> getCandidates(string kGram, string path)
        {
            path = Path.GetFullPath(path);

            string kGramTableBin = Path.Join(path, "kGramTable.bin");
            string candidatesBin = Path.Join(path, "candidates.bin");


            using (BinaryReader binaryReader = new BinaryReader(File.Open(kGramTableBin, FileMode.Open)))
            {
                binarySearchCandidates(binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length)), kGram);
            }
            return new List<string>();
        }


        private float binarySearchCandidates(byte[] v, string kGram, string path)
        {

            if (v.Length > 16)
            {
                int chunck = v.Length / 16;
                chunck = chunck / 2;

                byte[] currentChunck = v.Skip(chunck * 16).Take(8).ToArray();






                binarySearchCandidates(v.Take((chunck) * 16).ToArray(), kGram, path);
                binarySearchCandidates(v.Skip((chunck + 1) * 16).ToArray(), kGram, path);
            }
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
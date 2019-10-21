using System.IO;
using Search.Document;
using System.Collections.Generic;
namespace Search.Index
{
    public class DiskKGramWriter
    {

        public void WriteKGram(KGram kGram, string path)
        {
            //Generate full path
            path = Path.GetFullPath(path);
            this.WriteKGramTableBin(WriteKGramBin(kGram, path), WriteCandidatesBin(kGram, path), path);
        }

        private List<long> WriteCandidatesBin(KGram kGram, string path)
        {
            List<long> candidatesPositions = new List<long>();
            path = Path.Join(path, "KGram.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (string key in kGram.GetKGramMap().Keys){
                    
                }
            }
            return candidatesPositions;
        }

        private List<long> WriteKGramBin(KGram kGram, string path)
        {
        }
        private void WriteKGramTableBin(List<long> kGramPositions, List<long> CandidatesPositions, string path)
        {

        }
    }
}
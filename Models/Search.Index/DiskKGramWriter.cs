using System.IO;
using Search.Document;
using System.Collections.Generic;
namespace Search.Index
{
    /// <summary>
    /// Write KGram only disk
    /// </summary>
    public class DiskKGramWriter
    {
        /// <summary>
        /// Write KGram map to disk
        /// </summary>
        /// <param name="kGram">KGram object</param>
        /// <param name="path">where should kGram be written to</param>
        public void WriteKGram(KGram kGram, string path)
        {
            //Generate full path
            path = Path.GetFullPath(path);
            this.WriteKGramTableBin(WriteKGramBin(kGram, path), WriteCandidatesBin(kGram, path), path);
        }

        /// <summary>
        /// Write candidates to bin file
        /// </summary>
        /// <param name="kGram">KGram object</param>
        /// <param name="path">where should candidates bin be written to</param>
        /// <returns>Starting positions of each candidates</returns>
        private List<long> WriteCandidatesBin(KGram kGram, string path)
        {
            List<long> candidatesPositions = new List<long>();
            path = Path.Join(path, "candidates.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (List<string> candidates in kGram.GetKGramMap().Values)
                {
                    //TODO:
                }
            }
            return candidatesPositions;
        }

        /// <summary>
        /// Write KGram to bin file
        /// </summary>
        /// <param name="kGram">KGram object</param>
        /// <param name="path">where should KGram bin be written to</param>
        /// <returns></returns>
        private List<long> WriteKGramBin(KGram kGram, string path)
        {
            List<long> kGramPositions = new List<long>();
            path = Path.Join(path, "kgram.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (string kgram in kGram.GetKGramMap().Keys)
                {
                    kGramPositions.Add(b.BaseStream.Position);
                    b.Write(kgram);
                }
            }
            return kGramPositions;
        }

        /// <summary>
        /// Write relationship between KGram bin and candidates bin
        /// </summary>
        /// <param name="kGramPositions">Starting positions of all kGram</param>
        /// <param name="CandidatesPositions">Starting position of all candidates</param>
        /// <param name="path">where should KGramTable bin be written to</param>
        private void WriteKGramTableBin(List<long> kGramPositions, List<long> CandidatesPositions, string path)
        {

        }
    }
}
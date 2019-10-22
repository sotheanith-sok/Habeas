using System.IO;
using Search.Document;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System;
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
            this.WriteMiniKGramTableBin(WriteMiniKGramBin(kGram, path), WriteMiniCandidatesBin(kGram, path), path);
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
                foreach (ReadOnlyCollection<string> candidates in kGram.GetKGramMap().Values)
                {
                    candidatesPositions.Add(b.BaseStream.Position);
                    b.Write(Encoding.UTF8.GetBytes(string.Join(" ", candidates)));
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
            path = Path.Join(path, "kGram.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (string kgram in kGram.GetKGramMap().Keys)
                {
                    kGramPositions.Add(b.BaseStream.Position);
                    b.Write(Encoding.UTF8.GetBytes(kgram));
                }
            }
            return kGramPositions;
        }

        /// <summary>
        /// Write relationship between KGram bin and candidates bin
        /// </summary>
        /// <param name="kGramPositions">Starting positions of all kGram</param>
        /// <param name="candidatesPositions">Starting position of all candidates</param>
        /// <param name="path">where should KGramTable bin be written to</param>
        private void WriteKGramTableBin(List<long> kGramPositions, List<long> candidatesPositions, string path)
        {
            path = Path.GetFullPath(path + "kGramTable.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                for (int i = 0; i < kGramPositions.Count; i++)
                {
                    b.Write(kGramPositions[i]);
                    b.Write(candidatesPositions[i]);
                }
            }
        }

        /// <summary>
        /// Write mini kgram candidates to bin file
        /// </summary>
        /// <param name="kGram">KGram object</param>
        /// <param name="path">where should candidates bin be written to</param>
        /// <returns>Starting positions of each candidates</returns>
        private List<long> WriteMiniCandidatesBin(KGram kGram, string path)
        {
            List<long> miniCandidatesPositions = new List<long>();
            path = Path.Join(path, "miniCandidates.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (ReadOnlyCollection<string> candidates in kGram.GetMiniKGramMap().Values)
                {
                    miniCandidatesPositions.Add(b.BaseStream.Position);
                    b.Write(Encoding.UTF8.GetBytes(string.Join(" ", candidates)));
                }
            }
            return miniCandidatesPositions;
        }

        /// <summary>
        /// Write mini kGram to bin file
        /// </summary>
        /// <param name="kGram">KGram object</param>
        /// <param name="path">where should KGram bin be written to</param>
        /// <returns></returns>
        private List<long> WriteMiniKGramBin(KGram kGram, string path)
        {
            List<long> miniKGramPositions = new List<long>();
            path = Path.Join(path, "miniKGram.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (string kgram in kGram.GetMiniKGramMap().Keys)
                {
                    miniKGramPositions.Add(b.BaseStream.Position);
                    b.Write(Encoding.UTF8.GetBytes(kgram));
                }
            }
            return miniKGramPositions;
        }

        /// <summary>
        /// Write relationship between mini kgram bin and kgram candidates bin
        /// </summary>
        /// <param name="miniKGramPositions">Starting positions of all kGram</param>
        /// <param name="miniCandidatesPositions">Starting position of all candidates</param>
        /// <param name="path">where should KGramTable bin be written to</param>
        private void WriteMiniKGramTableBin(List<long> miniKGramPositions, List<long> miniCandidatesPositions, string path)
        {
            path = Path.GetFullPath(path + "miniKGramTable.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                for (int i = 0; i < miniKGramPositions.Count; i++)
                {
                    b.Write(miniKGramPositions[i]);
                    b.Write(miniCandidatesPositions[i]);
                }
            }
        }
    }
}
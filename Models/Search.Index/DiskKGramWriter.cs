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
        public void WriteKGram(SortedDictionary<string, List<string>> kGram, SortedDictionary<string, List<string>> miniKGram, string path)
        {
            //Generate full path
            path = Path.GetFullPath(path);
            this.WriteKGramTableBin(WriteKGramBin(kGram, path), WriteCandidatesBin(kGram, path), path);
            this.WriteMiniKGramTableBin(WriteMiniKGramBin(miniKGram, path), WriteMiniCandidatesBin(miniKGram, path), path);
        }

        /// <summary>
        /// Write candidates to bin file
        /// </summary>
        /// <param name="kGram">KGram object</param>
        /// <param name="path">where should candidates bin be written to</param>
        /// <returns>Starting positions of each candidates</returns>
        private List<long> WriteCandidatesBin(SortedDictionary<string, List<string>> kGram, string path)
        {
            Console.WriteLine("Writing KGram candidates bin...");
            List<long> candidatesPositions = new List<long>();
            path = Path.Join(path, "candidates.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (List<string> candidates in kGram.Values)
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
        private List<long> WriteKGramBin(SortedDictionary<string, List<string>> kGram, string path)
        {
            Console.WriteLine("Writing KGram bin...");
            List<long> kGramPositions = new List<long>();
            path = Path.Join(path, "kGram.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (string kgram in kGram.Keys)
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
            Console.WriteLine("Writing KGram table bin...");
            path = Path.Join(path, "kGramTable.bin");
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
        /// Write mini KGram candidates to bin file
        /// </summary>
        /// <param name="miniKGram">KGram object</param>
        /// <param name="path">where should candidates bin be written to</param>
        /// <returns>Starting positions of each candidates</returns>
        private List<long> WriteMiniCandidatesBin(SortedDictionary<string, List<string>> miniKGram, string path)
        {
            Console.WriteLine("Writing mini KGram candidates bin...");
            List<long> miniCandidatesPositions = new List<long>();
            path = Path.Join(path, "miniCandidates.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (List<string> candidates in miniKGram.Values)
                {
                    miniCandidatesPositions.Add(b.BaseStream.Position);
                    b.Write(Encoding.UTF8.GetBytes(string.Join(" ", candidates)));
                }
            }
            return miniCandidatesPositions;
        }

        /// <summary>
        /// Write mini KGram to bin file
        /// </summary>
        /// <param name="miniKGram">KGram object</param>
        /// <param name="path">where should KGram bin be written to</param>
        /// <returns></returns>
        private List<long> WriteMiniKGramBin(SortedDictionary<string, List<string>> miniKGram, string path)
        {
            Console.WriteLine("Writing mini KGram bin...");
            List<long> miniKGramPositions = new List<long>();
            path = Path.Join(path, "miniKGram.bin");
            using (BinaryWriter b = new BinaryWriter(File.Create(path)))
            {
                foreach (string kgram in miniKGram.Keys)
                {
                    miniKGramPositions.Add(b.BaseStream.Position);
                    b.Write(Encoding.UTF8.GetBytes(kgram));
                }
            }
            return miniKGramPositions;
        }

        /// <summary>
        /// Write relationship between mini KGram bin and KGram candidates bin
        /// </summary>
        /// <param name="miniKGramPositions">Starting positions of all kGram</param>
        /// <param name="miniCandidatesPositions">Starting position of all candidates</param>
        /// <param name="path">where should KGramTable bin be written to</param>
        private void WriteMiniKGramTableBin(List<long> miniKGramPositions, List<long> miniCandidatesPositions, string path)
        {
            Console.WriteLine("Writing mini KGram table bin");
            path = Path.Join(path, "miniKGramTable.bin");
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
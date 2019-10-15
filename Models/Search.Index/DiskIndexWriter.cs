
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Search.Query;

namespace Search.Index
{
    /// <summary>
    /// Write index to the disk
    /// </summary>
    public class DiskIndexWriter
    {
        /// <summary>
        /// Writes the index on disk with 3 files
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="filePath">the absolute path to save the index files</param>
        public void WriteIndex(IIndex index, string filePath)
        {
            new BinaryWriter(File.Open("Vocab.bin", FileMode.Create));
            new BinaryWriter(File.Open("Postings.bin", FileMode.Create));
            IReadOnlyList<string> vocabulary = index.GetVocabulary();
            long termStart;
            long postingStart;

            foreach (string term in vocabulary)
            {
                termStart = WriteVocab(term);
                postingStart = WritePostings(index.GetPostings(term), filePath);
                WriteVocabTable(termStart, postingStart);
            }
        }

        /// <summary>
        /// Writes a posting list of a term to postings.bin
        /// </summary>
        /// <param name="postings">the posting list to write</param>
        /// <returns>the starting byte position of the posting list in postings.bin</returns>
        public long WritePostings(IList<Posting> postings, string folderPath) {
            string fileName = "postings.bin";
            long startByte;

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
            {
                startByte = writer.BaseStream.Length;
                foreach(Posting p in postings)
                {
                    //Write docID
                    writer.Write(p.DocumentId);
                    writer.Write(' ');

                    //Write positions
                    foreach(int pos in p.Positions)
                    {
                        writer.Write(pos);
                        writer.Write(' ');
                    }
                }
            }
            return startByte;
        }

        /// <summary>
        /// Writes the vocab.bin file
        /// </summary>
        public long WriteVocab(string vocabulary)
        {
            long finalLong;
            using (BinaryWriter writer = new BinaryWriter(File.Open("Vocab.bin", FileMode.Append)))
            {
                finalLong=writer.BaseStream.Length;
                writer.Write(vocabulary);
            }
        return finalLong; 
        }

        /// <summary>
        /// Writes the starting byte of term and the starting byte of posting list to vocabTable.bin
        /// </summary>
        /// <param name="termStart">the byte position of a term in vocab.bin</param>
        /// <param name="postingStart">the byte position of a posting list in postings.bin</param>
        public void WriteVocabTable(long termStart, long postingStart)
        {
            string fileName = "vocabTable.bin";
            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
            {
                writer.Write(termStart);
                writer.Write(postingStart);
            }
        }
    }
}

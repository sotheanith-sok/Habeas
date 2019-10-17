
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
        /// Writes the index on disk as three files
        /// 1) vocab.bin, 2) postings.bin, 3) vocabTable.bin
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to the directory where the index files be saved</param>
        public void WriteIndex(IIndex index, string dirPath)
        {
            string vocabFilePath = dirPath + "vocab.bin";
            string postingFilePath = dirPath + "postings.bin";
            string vocabTableFilePath = dirPath + "vocabTable.bin";
            //Create the files (Overwrite any existed files)
            File.Create(vocabFilePath).Dispose();
            File.Create(postingFilePath).Dispose();
            File.Create(vocabTableFilePath).Dispose();
            //Prep the binary writer with append file mode
            BinaryWriter vocabWriter = new BinaryWriter(File.Open(vocabFilePath, FileMode.Append));
            BinaryWriter postingWriter = new BinaryWriter(File.Open(postingFilePath, FileMode.Append));
            BinaryWriter vocabTableWriter = new BinaryWriter(File.Open(vocabTableFilePath, FileMode.Append));
            
            IReadOnlyList<string> vocabulary = index.GetVocabulary();
            long termStart;     //the byte position of where a term starts in 'vocab.bin' on disk
            long postingStart;  //the byte position of where the posting list starts in 'postings.bin' on disk
            foreach (string term in vocabulary)
            {
                termStart = WriteVocab(term, vocabWriter);
                postingStart = WritePostings(index.GetPostings(term), postingWriter);
                WriteVocabTable(termStart, postingStart, vocabTableWriter);
            }
        }

        /// <summary>
        /// Writes a posting list of a term to postings.bin
        /// </summary>
        /// <param name="postings">the posting list to write</param>
        /// <returns>the starting byte position of the posting list in postings.bin</returns>
        public long WritePostings(IList<Posting> postings, BinaryWriter writer) {
            long startByte;

            // using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
            // {
                startByte = writer.BaseStream.Length;
                foreach(Posting p in postings)
                {
                    //TODO: Write docID with gap
                    writer.Write(p.DocumentId);         //4byte integer per docID

                    //TODO: Write positions with gap
                    foreach(int pos in p.Positions)
                    {
                        writer.Write(pos);              //4byte integer per position
                    }
                }
            // }
            return startByte;
        }

        /// <summary>
        /// Writes the vocab.bin file
        /// </summary>
        public long WriteVocab(string term, BinaryWriter writer)
        {
            long startByte;
            // using (BinaryWriter writer = new BinaryWriter(File.Open("Vocab.bin", FileMode.Append)))
            // {
                startByte = writer.BaseStream.Length;
                writer.Write(term);
            // }
            return startByte; 
        }

        /// <summary>
        /// Writes each starting byte of a term and the posting list to vocabTable.bin
        /// </summary>
        /// <param name="termStart">the byte position of a term in vocab.bin</param>
        /// <param name="postingStart">the byte position of a posting list in postings.bin</param>
        public void WriteVocabTable(long termStart, long postingStart, BinaryWriter writer)
        {
            // using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Append)))
            // {
                writer.Write(termStart);        //each starting address with 8-byte integer (long)
                writer.Write(postingStart);     //each starting address with 8-byte integer (long)
            // }
        }
    }
}

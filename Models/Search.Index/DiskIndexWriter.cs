
using System;
using System.Collections.Generic;
using System.IO;

namespace Search.Index
{
    /// <summary>
    /// Write index on the disk
    /// </summary>
    public class DiskIndexWriter
    {
        /// <summary>
        /// Writes the index on disk as three files
        /// 1) vocab.bin, 2) postings.bin, 3) vocabTable.bin
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to the directory where the index files to be saved</param>
        public void WriteIndex(IIndex index, string dirPath)
        {
            Console.WriteLine($"\nWriting the index ({index.GetVocabulary().Count} terms) in '{dirPath}'");

            string vocabFilePath = dirPath + "vocab.bin";
            string postingFilePath = dirPath + "postings.bin";
            string vocabTableFilePath = dirPath + "vocabTable.bin";

            try
            {
                //Create the files (Overwrite any existed files)
                Directory.CreateDirectory(dirPath);
                File.Create(vocabFilePath).Dispose();
                File.Create(postingFilePath).Dispose();
                File.Create(vocabTableFilePath).Dispose();

                //Open files in 'append' mode with the binary writers
                BinaryWriter vocabWriter = new BinaryWriter(File.Open(vocabFilePath, FileMode.Append));
                BinaryWriter postingWriter = new BinaryWriter(File.Open(postingFilePath, FileMode.Append));
                BinaryWriter vocabTableWriter = new BinaryWriter(File.Open(vocabTableFilePath, FileMode.Append));
                
                //Write the index in three files term by term
                IReadOnlyList<string> vocabulary = index.GetVocabulary();
                long termStart;     //the byte position of where a term starts in 'vocab.bin'
                long postingStart;  //the byte position of where the posting list of a term starts in 'postings.bin'
                
                foreach (string term in vocabulary)
                {
                    termStart = WriteVocab(term, vocabWriter);
                    postingStart = WritePostings(index.GetPostings(term), postingWriter);
                    WriteVocabTable(termStart, postingStart, vocabTableWriter);
                }
                Console.WriteLine("Finished writing the index");
                Console.WriteLine($"vocab.bin       {vocabWriter.BaseStream.Length} bytes");
                Console.WriteLine($"postings.bin    {postingWriter.BaseStream.Length} bytes");
                Console.WriteLine($"vocabTable.bin  {vocabTableWriter.BaseStream.Length} bytes\n");

                //Close the files
                vocabWriter.Dispose();
                postingWriter.Dispose();
                vocabTableWriter.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }

        /// <summary>
        /// Writes a posting list of a term to postings.bin
        /// </summary>
        /// <param name="postings">the posting list to write</param>
        /// <param name="writer">a binary writer with append file mode</param>
        /// <returns>the starting byte position of the posting list in postings.bin</returns>
        public long WritePostings(IList<Posting> postings, BinaryWriter writer)
        {
            long startByte = writer.BaseStream.Length;

            int previousDocID = 0;
            foreach(Posting p in postings)
            {
                //Write docID using gap
                writer.Write(p.DocumentId - previousDocID); //4byte integer per docID

                //Write positions using gap
                int previousPos = 0;
                foreach(int pos in p.Positions)
                {
                    writer.Write(pos - previousPos);        //4byte integer per position
                    previousPos = pos;
                }

                previousDocID = p.DocumentId;
            }

            return startByte;

        }

        /// <summary>
        /// Writes a term to vocab.bin
        /// </summary>
        /// <param name="term">a term to write</param>
        /// <param name="writer">a binary writer with append file mode</param>
        /// /// <returns>the starting byte position of the term in vocab.bin</returns>
        public long WriteVocab(string term, BinaryWriter writer)
        {
            long startByte = writer.BaseStream.Length;

            writer.Write(term);

            return startByte; 

        }

        /// <summary>
        /// Writes each starting byte of a term and the posting list to vocabTable.bin
        /// </summary>
        /// <param name="termStart">the byte position of a term in vocab.bin</param>
        /// <param name="postingStart">the byte position of a posting list in postings.bin</param>
        /// /// <param name="writer">a binary writer with append file mode</param>
        public void WriteVocabTable(long termStart, long postingStart, BinaryWriter writer)
        {
            writer.Write(termStart);        //each starting address with 8-byte integer (long)
            writer.Write(postingStart);     //each starting address with 8-byte integer (long)

        }
    }
}

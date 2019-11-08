
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
        //TODO: Make all methods except WriteIndex() private later after testing

        /// <summary>
        /// Writes the index on disk as three files
        /// 1) vocab.bin, 2) postings.bin, 3) vocabTable.bin
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to the directory where the index files to be saved</param>
        public void WriteIndex(IIndex index, string dirPath)
        {
            Console.WriteLine($"\nWriting the index ({index.GetVocabulary().Count} terms) in '{dirPath}'");

            String dirIndexPath = dirPath+"/index/";

            Directory.CreateDirectory(dirIndexPath);

            List<long> vocabStartBytes = WriteVocab(index, dirIndexPath);
            List<long> postingsStartBytes = WritePostings(index, dirIndexPath);
            WriteVocabTable(vocabStartBytes, postingsStartBytes, dirIndexPath);

            List<long> docWeightsStartBytes = WriteDocWeights(index, dirIndexPath);

            Console.WriteLine("Finished writing the index on disk.\n\n");

        }

        /// <summary>
        /// Writes the term frequency and postings of each term in an index to postings.bin
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to a directory where 'postings.bin' be saved</param>
        /// <returns>the list of starting byte positions of each posting lists in postings.bin</returns>
        public List<long> WritePostings(IIndex index, string dirPath)
        {
            string filePath = dirPath + "postings.bin";
            File.Create(filePath).Dispose();

            List<long> startBytes = new List<long>();
            IReadOnlyList<string> vocabulary = index.GetVocabulary();

            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
            {
                foreach (string term in vocabulary)
                {
                    startBytes.Add(writer.BaseStream.Length);       //add start byte positions of each posting list
                    IList<Posting> postings = index.GetPositionalPostings(term);

                    //1. Write document frequency (# of postings)
                    writer.Write(postings.Count);

                    int previousDocID = 0;
                    foreach (Posting p in postings)
                    {
                        //2. Write docID using gap
                        writer.Write(p.DocumentId - previousDocID); //4byte integer per docID

                        List<int> positions = p.Positions;

                        //3. Write term frequency (# of positions)
                        writer.Write(positions.Count);              //4byte integer per term frequency

                        //4. Write positions using gap
                        int previousPos = 0;
                        foreach (int pos in positions)
                        {
                            writer.Write(pos - previousPos);        //4byte integer per position
                            previousPos = pos;
                        }

                        previousDocID = p.DocumentId;
                    }
                }
                Console.WriteLine($"postings.bin    {writer.BaseStream.Length} bytes");
            }
            return startBytes;

        }

        /// <summary>
        /// Writes the entire terms in the vocabulary of an index to vocab.bin
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to a directory where 'vocab.bin' be saved</param>
        /// <returns>the list of starting byte positions of each term in vocab.bin</returns>
        public List<long> WriteVocab(IIndex index, string dirPath)
        {
            string filePath = dirPath + "vocab.bin";
            File.Create(filePath).Dispose();

            List<long> startBytes = new List<long>();
            IReadOnlyList<string> vocabulary = index.GetVocabulary();

            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
            {
                foreach (string term in vocabulary)
                {
                    startBytes.Add(writer.BaseStream.Length);
                    writer.Write(term);
                }
                Console.WriteLine($"vocab.bin       {writer.BaseStream.Length} bytes");
            }
            return startBytes;

        }

        /// <summary>
        /// Writes each starting byte positions of terms and the posting lists to vocabTable.bin
        /// </summary>
        /// <param name="termStartBytes">the list of byte positions of terms in vocab.bin</param>
        /// <param name="postingsStartBytes">the list of byte positions of posting lists in postings.bin</param>
        /// <returns>the list of starting byte positions of each term in vocab.bin</returns>
        public void WriteVocabTable(List<long> termStartBytes, List<long> postingsStartBytes, string dirPath)
        {
            string filePath = dirPath + "vocabTable.bin";
            File.Create(filePath).Dispose();
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
            {
                if (termStartBytes.Count != postingsStartBytes.Count)
                {
                    Console.WriteLine("the size of termStartBytes and postingsStartBytes are not matching!");
                }

                for (int i = 0; i < termStartBytes.Count; i++)
                {
                    writer.Write(termStartBytes[i]);        //each starting address with 8-byte integer (long)
                    writer.Write(postingsStartBytes[i]);    //each starting address with 8-byte integer (long)
                }

                Console.WriteLine($"vocabTable.bin  {writer.BaseStream.Length} bytes");
            }

        }

        ///<sumary>
        /// Writes 8-byte values of document weights to docWeights.bin 
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to a directory where 'docWeights.bin' be saved</param>
        /// <returns>the list of starting byte positions of each doc weight in docWeights.bin</returns>
        public List<long> WriteDocWeights(IIndex index, string dirPath)
        {
            string filePath = dirPath + "docWeights.bin";
            File.Create(filePath).Dispose();
            List<long> startBytes = new List<long>();

            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
            {
                foreach (double weight in index.GetAllDocWeights())
                {
                    startBytes.Add(writer.BaseStream.Length);
                    writer.Write(BitConverter.DoubleToInt64Bits(weight));
                }

                Console.WriteLine($"docWeights.bin  {writer.BaseStream.Length} bytes");
            }

            return startBytes;
        }

    }
}

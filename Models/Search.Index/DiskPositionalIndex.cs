using System.Collections.Generic;
using System.Linq;
using Search.Query;
using System;
using System.IO;

namespace Search.Index
{
    /// <summary>
    /// 
    /// </summary>
    public class DiskPositionalIndex : IIndex, IDisposable
    {

        BinaryReader VocabReader;
        BinaryReader PostingReader;
        BinaryReader VocabTableReader;

        /// <summary>
        /// 
        /// </summary>
        public DiskPositionalIndex(string FolderPath)
        {

            String VocabPath = FolderPath + "vocab.bin";
            String PostingPath = FolderPath + "postings.bin";
            String VocabTablePath = FolderPath + "vocabTable.bin";

            VocabReader = new BinaryReader(File.Open(VocabPath, FileMode.Open));
            PostingReader = new BinaryReader(File.Open(PostingPath, FileMode.Open));
            VocabTableReader = new BinaryReader(File.Open(VocabTablePath, FileMode.Open));

        }

        /// <summary>
        /// Gets Postings of a given term from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {


            return new List<Posting>();

        }

        /// <summary>
        /// Gets Postings of a given list of terms from index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {

            return new List<Posting>();
        }

        /// <summary>
        /// Gets Postings of a given term from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostingsPositional(string term)
        {


            return new List<Posting>();

        }

        /// <summary>
        /// Gets Postings of a given list of terms from index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostingsPositional(List<string> terms)
        {

            return new List<Posting>();
        }

        /// <summary>
        /// Gets a sorted list of all vocabularies from index.
        /// </summary>
        public IReadOnlyList<string> GetVocabulary()
        {
            List<string> finalList = new List<String>();
            while ()
            {
                finalList.Add(VocabReader.ReadString());
            }
            return finalList;
        }

        public int getTermCount()
        {
            //ToDo: make it work
            //return mVocabTable.length / 2;
            long doubleLength = VocabTableReader.BaseStream.Length;
            return 5;        
        }

        public void Dispose()
        {
            VocabReader?.Dispose();
            PostingReader?.Dispose();
            VocabTableReader?.Dispose();
        }
    }

}

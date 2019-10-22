using System.Collections.Generic;
using System.Linq;
using Search.Query;
using System;
using System.IO;

namespace Search.Index
{
    /// <summary>
    /// Implements a Positional Inverted Index in a hash map.
    /// key: term, value: list of Posting.
    /// e.g. term -> { (doc1, [pos1, pos2]), (doc2, [pos1, pos3, pos4]), ... }
    /// </summary>
    public class DiskPositionalIndex : IIndex
    {

        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public DiskPositionalIndex(string FolderPath)
        {
            //FolderPath is a string leading to the folder with the different bin files
        BinaryReader reader;
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
            return new List<string>();
        }

        /// <summary>
        /// Adds a term into the index with its docId and position.
        /// </summary>
        /// <param name="term">a processed string to be added</param>
        /// <param name="docID">the document id in which the term is coming from</param>
        /// <param name="position">the position of the term within the document</param>
        public void AddTerm(string term, int docID, int position)
        {
          

        }


        public Posting GetLastPostingItem(string term)
        {
            return new Posting(0, new List<int>());
        }
    }

}

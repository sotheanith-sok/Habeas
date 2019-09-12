using System.Collections.Generic;
using System;
namespace Search.Index
{
    /// <summary>
    /// Represents a document that is saved as a simple text file in the local file system.
    /// </summary>
    public class TermDocumentInvertedIndex : IIndex
    {
        //Hashmap used to store inverted index sorted key by alphabetical order.
        private readonly SortedDictionary<string, List<Posting>> invertedIndex;

        /// <summary>
        /// The constructor used to build this object.
        /// </summary>
        public TermDocumentInvertedIndex()
        {
            invertedIndex = new SortedDictionary<string, List<Posting>>();
        }

        /// <summary>
        /// Get Posting from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        public IList<Posting> GetPostings(string term)
        {
            if (invertedIndex.ContainsKey(term))
            {
                return invertedIndex[term];
            }
            else
            {
                return new List<Posting>();
            }

        }

        /// <summary>
        /// Get all vocabularies in sorted order.
        /// </summary>
        public IReadOnlyList<string> GetVocabulary()
        {
            return new List<string>(invertedIndex.Keys);
        }

        /// <summary>
        /// Add term into inverted index.
        /// </summary>
        /// <param name="term">a processed string to be added</param>
        /// <param name="documentID">the document id in which the term is coming from</param>
        public void AddTerm(string term, int documentID)
        {

            if (invertedIndex.ContainsKey(term))
            {
                List<Posting> postingList = invertedIndex[term];

                if ((postingList[postingList.Count - 1]).DocumentId != documentID)
                {
                    postingList.Add(new Posting(documentID));
                }
            }
            else
            {
                invertedIndex.Add(term, new List<Posting>() { new Posting(documentID) });
            }
        }

    }


}

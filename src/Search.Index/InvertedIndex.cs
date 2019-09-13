using System.Collections.Generic;
using System.Linq;

namespace Search.Index
{
    /// <summary>
    /// Implements an inverted index in a hash map (key: term, value: list of postings)
    /// </summary>
    public class InvertedIndex : IIndex
    {
        //Hashmap is used to store inverted index. O(1)
        //Dictionary in C# is equivalent to HashMap in Java.
        private readonly Dictionary<string, List<Posting>> hashMap;

        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public InvertedIndex()
        {
            hashMap = new Dictionary<string, List<Posting>>();
        }

        /// <summary>
        /// Gets Postings from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        public IList<Posting> GetPostings(string term)
        {
            if (hashMap.ContainsKey(term)) {
                return hashMap[term];
            } else {
                return new List<Posting>();
            }

        }

        /// <summary>
        /// Gets a sorted list of all vocabularies from index.
        /// </summary>
        public IReadOnlyList<string> GetVocabulary()
        {
            List<string> vocabulary = hashMap.Keys.ToList();
            vocabulary.Sort();
            return vocabulary;
        }

        /// <summary>
        /// Adds a term into the index.
        /// </summary>
        /// <param name="term">a processed string to be added</param>
        /// <param name="documentID">the document id in which the term is coming from</param>
        public void AddTerm(string term, int documentID)
        {
            //Check if inverted index contains the search term (key)
            if (hashMap.ContainsKey(term)) {
                //Check if the docId is in the List<Posting> (value)
                //the list is naturally in order of docId
                if (hashMap[term].Last().DocumentId != documentID){
                    hashMap[term].Add(new Posting(documentID));
                }
            } else {
                hashMap.Add(term, new List<Posting>() { new Posting(documentID) });
            }
        }

    }

}

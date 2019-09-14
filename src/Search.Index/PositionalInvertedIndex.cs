using System.Collections.Generic;
using System.Linq;

namespace Search.Index
{
    /// <summary>
    /// Implements a Positional Inverted Index in a hash map
    /// key: a term
    /// value: a list of PositionalPosting
    /// </summary>
    public class PositionalInvertedIndex : IIndex
    {
        //Hashmap is used to store the index. O(1)
        //Dictionary in C# is equivalent to HashMap in Java.
        private readonly Dictionary<string, List<PositionalPosting>> hashMap;

        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public PositionalInvertedIndex()
        {
            hashMap = new Dictionary<string, List<PositionalPosting>>();
        }

        /// <summary>
        /// Gets PositionalPostings from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a PositionalPosting</return>
        public IList<PositionalPosting> GetPositionalPostings(string term)
        {
            if (hashMap.ContainsKey(term)) {
                return hashMap[term];
            } else {
                return new List<PositionalPosting>();
            }
        }
        
        //TODO: either replace GetPositionalPostings or delete this
        public IList<Posting> GetPostings(string term)
        { 
            return null; 
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
        /// Adds a term into the index with its docId and position.
        /// </summary>
        /// <param name="term">a processed string to be added</param>
        /// <param name="documentID">the document id in which the term is coming from</param>
        /// <param name="position">the position of the term within the document</param>
        public void AddTerm(string term, int documentID, int position)
        {
            
            // //TODO: handle position
            // //Check if inverted index contains the search term (key)
            // if (hashMap.ContainsKey(term)) {
            //     //Check if the docId is in the List<Posting> (value)
            //     //the list is naturally in order of docId
            //     if (hashMap[term].Last().DocumentId != documentID){
            //         hashMap[term].Add(new PositionalPosting(documentID, position));
            //     }
            // } else {

            //     List<PositionalPosting> postingList = new List<PositionalPosting>();
            //     hashMap.Add(term, new List<PositionalPosting>() { new PositionalPosting(documentID) });
            //}
        }

    }

}

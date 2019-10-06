using System.Collections.Generic;
using System.Linq;
using Search.Query;

namespace Search.Index
{
    /// <summary>
    /// Implements a Positional Inverted Index in a hash map.
    /// key: term, value: list of Posting.
    /// e.g. term -> { (doc1, [pos1, pos2]), (doc2, [pos1, pos3, pos4]), ... }
    /// </summary>
    public class PositionalInvertedIndex : IIndex
    {
        //Hashmap is used to store the index. O(1)
        //Dictionary in C# is equivalent to HashMap in Java.
        private readonly Dictionary<string, List<Posting>> hashMap;

        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public PositionalInvertedIndex()
        {
            hashMap = new Dictionary<string, List<Posting>>();
        }

        /// <summary>
        /// Gets Postings of a given term from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {
            if (hashMap.ContainsKey(term)) {
                return hashMap[term];
            } else {
                return new List<Posting>();
            }
        }

        /// <summary>
        /// Gets Postings of a given list of terms from index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {
			List<IList<Posting>> postingLists = new List<IList<Posting>>();
			foreach(string term in terms) {
				postingLists.Add( GetPostings(term) );
			}
			return Merge.OrMerge(postingLists);
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
        /// <param name="docID">the document id in which the term is coming from</param>
        /// <param name="position">the position of the term within the document</param>
        public void AddTerm(string term, int docID, int position)
        {
            //Check if inverted index contains the term (key)
            if (hashMap.ContainsKey(term)) {
                //Check if the document of the term is in the posting list
                Posting lastPosting = hashMap[term].Last();
                if(lastPosting.DocumentId == docID){
                    //Add a position to the posting
                    lastPosting.Positions.Add(position);
                } else {
                    //Create a posting with (docID & position) to the posting list
                    hashMap[term].Add(new Posting(docID, new List<int>{position}));
                }
            } else {
                //Add term and a posting (docID & position) to the hashmap
                List<Posting> postingList = new List<Posting>();
                postingList.Add(new Posting(docID, new List<int>{position}));
                hashMap.Add(term, postingList);
            }

        }


        public Posting GetLastPostingItem(string term)
        {
           return hashMap[term].Last();
        }
    }

}

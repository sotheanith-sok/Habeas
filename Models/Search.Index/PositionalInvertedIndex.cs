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
    public class PositionalInvertedIndex : IIndex
    {
        //Hashmap is used to store the index. O(1)
        //Dictionary in C# is equivalent to HashMap in Java.
        private readonly Dictionary<string, List<Posting>> hashMap;

        //HashMap used to store termFrequency of current Document
        private readonly Dictionary<string, int> termFrequency;


        private static List<double> calculatedDocWeights;


        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public PositionalInvertedIndex()
        {
            hashMap = new Dictionary<string, List<Posting>>();
            termFrequency = new Dictionary<string, int>();
            calculatedDocWeights = new List<double>();
        }

        /// <summary>
        /// Gets Postings of a given term from in-memory index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {
            //TODO: change to retrieve postings without regard to position for ranked retrieval?
            if (hashMap.ContainsKey(term))
            {
                return hashMap[term];
            }
            else
            {
                return new List<Posting>();
            }
        }

        /// <summary>
        /// Gets Postings of a given list of terms from in-memory index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {
            //TODO: change to retrieve postings without regard to position for ranked retrieval?
            List<IList<Posting>> postingLists = new List<IList<Posting>>();
            foreach (string term in terms)
            {
                postingLists.Add(GetPostings(term));
            }
            return Merge.OrMerge(postingLists);
        }


        /// <summary>
        /// Gets Postings of a given term from in-memory index.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        public IList<Posting> GetPositionalPostings(string term)
        {
            return GetPostings(term);
        }

        /// <summary>
        /// Gets Postings of a given list of terms from in-memory index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPositionalPostings(List<string> terms)
        {
            return GetPostings(terms);
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
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public Posting GetLastPostingItem(string term)
        {
            return hashMap[term].Last();
        }

        /// <summary>
        /// Adds a term into the index with its docId and position.
        /// </summary>
        /// <param name="term">a processed string to be added</param>
        /// <param name="docID">the document id in which the term is coming from</param>
        /// <param name="position">the position of the term within the document</param>
        public void AddTerm(string term, int docID, int position)
        {
            //ChangeFrequency
            UpdateTermFrequencyForDoc(term);

            //Check if inverted index contains the term (key)
            if (hashMap.ContainsKey(term))
            {
                //Check if the document of the term is in the posting list
                Posting lastPosting = hashMap[term].Last();
                if (lastPosting.DocumentId == docID)
                {
                    //Add a position to the posting
                    lastPosting.Positions.Add(position);
                }
                else
                {
                    //Create a posting with (docID & position) to the posting list
                    hashMap[term].Add(new Posting(docID, new List<int> { position }));
                }

            }
            else
            {

                //Add term and a posting (docID & position) to the hashmap
                List<Posting> postingList = new List<Posting>();
                postingList.Add(new Posting(docID, new List<int> { position }));
                hashMap.Add(term, postingList);

            }

        }

        /// <summary>
        /// Increases the instance of a term in a document in our Term Frequence HashMap
        /// </summary>
        /// <param name="term">Takes in the term that we want to update</param>
        public void UpdateTermFrequencyForDoc(string term)
        {

            if (termFrequency.ContainsKey(term))
            {
                termFrequency[term] += 1;
            }
            else
            {
                termFrequency.Add(term, 1);
            }

        }

        /// <summary>
        /// Applies the mathematical rule that we are using to calculate the document weight
        /// </summary>
        public void CalculateDocWeight()
        {
            double temp = 0;
            foreach (int value in termFrequency.Values)
            {
                temp = temp + Math.Pow((1 + Math.Log(value)), 2);
            }
            calculatedDocWeights.Add(Math.Sqrt(temp));
            //clear frequency map for next iteration of document
            termFrequency.Clear();
        }


        /// <summary>
        /// Gets all the document weights saved in memory
        /// </summary>
        /// <returns></returns>
        public IList<double> GetAllDocWeights()
        {
            return calculatedDocWeights;
        }


       
    }

}

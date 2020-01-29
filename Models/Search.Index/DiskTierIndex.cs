
using System.Collections.Generic;
using System.Linq;
using Search.Query;
using System;
using Search.OnDiskDataStructure;
using System.IO;
using Search.Document;
namespace Search.Index
{
    public class DiskTierIndex : IIndex
    {
        private IIndex regIndex;

        private Dictionary<string, List<MaxPriorityQueue.InvertedIndex>> tempTier1;
        private Dictionary<string, List<MaxPriorityQueue.InvertedIndex>> tempTier2;
        private Dictionary<string, List<MaxPriorityQueue.InvertedIndex>> tempTier3;


        private OnDiskDictionary<string, List<MaxPriorityQueue.InvertedIndex>> tier1;
        private OnDiskDictionary<string, List<MaxPriorityQueue.InvertedIndex>> tier2;
        private OnDiskDictionary<string, List<MaxPriorityQueue.InvertedIndex>> tier3;




        public DiskTierIndex(string path, IIndex index)
        {
            regIndex = index;
            tempTier1 = new Dictionary<string, List<MaxPriorityQueue.InvertedIndex>>();
            tempTier2 = new Dictionary<string, List<MaxPriorityQueue.InvertedIndex>>();
            tempTier3 = new Dictionary<string, List<MaxPriorityQueue.InvertedIndex>>();



            tier1 = new OnDiskDictionary<string, List<MaxPriorityQueue.InvertedIndex>>(path, "Tier1Index", new StringEncoderDecoder(), new InvertedIndexEncoderDecoder());
            tier2 = new OnDiskDictionary<string, List<MaxPriorityQueue.InvertedIndex>>(path, "Tier2Index", new StringEncoderDecoder(), new InvertedIndexEncoderDecoder());
            tier3 = new OnDiskDictionary<string, List<MaxPriorityQueue.InvertedIndex>>(path, "Tier3Index", new StringEncoderDecoder(), new InvertedIndexEncoderDecoder());


        }
        public void CreateTiers(Dictionary<string, List<Posting>> postingsMap)
        {

            Console.WriteLine("Creating Tier Indices-------------");
            IList<Posting> termPostings;
            IReadOnlyList<string> vocab = postingsMap.Keys.ToList();
            Console.WriteLine("Vocab Size: " + vocab.Count);

            foreach (string term in vocab)
            {

                //get postings for the term
                termPostings = postingsMap[term];


                //make priority queue

                MaxPriorityQueue tierQueue = new MaxPriorityQueue();
                foreach (Posting p in termPostings)
                {
                    tierQueue.MaxHeapInsert(p.Positions.Count, p.DocumentId);
                }

                int Whole = tierQueue.GetPriorityQueue().Count;
                //Create the Tiers using a priority queue
                List<MaxPriorityQueue.InvertedIndex> temp = tierQueue.RetrieveTier(50);
                int Tier1Amount = temp.Count;
                tempTier1.Add(term, temp);

                temp = tierQueue.RetrieveTier(50);
                int Tier2Amount = temp.Count;
                tempTier2.Add(term, temp);

                temp = tierQueue.RetrieveTier(100);
                int Tier3Amount = temp.Count;
                tempTier3.Add(term, temp);

                if (Whole > 2)
                {
                    Console.WriteLine(term + ": " + Whole + ", " + Tier1Amount + ", " + Tier2Amount + ", " + Tier3Amount + ", ");
                }

            }

            this.Save();

        }

        public void Save()
        {
            tier1.Replace(tempTier1);
            tier2.Replace(tempTier2);
            tier3.Replace(tempTier3);

            tempTier1.Clear();
            tempTier2.Clear();
            tempTier3.Clear();

        }
        public void Clear()
        {
            tier1.Clear();
            tier2.Clear();
            tier3.Clear();
        }
        public List<MaxPriorityQueue.InvertedIndex> GetPostingsFromTier(string term, int tierNumber = 1)
        {

            List<MaxPriorityQueue.InvertedIndex> result = new List<MaxPriorityQueue.InvertedIndex>();
            List<MaxPriorityQueue.InvertedIndex> temp = new List<MaxPriorityQueue.InvertedIndex>();
            switch (tierNumber)
            {
                case 1:
                    temp = tier1.Get(term);
                    
                    if (temp == null)
                    {
                        //Console.WriteLine(temp.Count);
                        return new List<MaxPriorityQueue.InvertedIndex>();
                    }
                    else
                    {
                        foreach (var index in temp)
                        {

                            int tf = index.GetTermFreq();
                            int id = index.GetDocumentId();
                            //Console.Write(id + " ");
                            Tuple<int, int> tempTuple = new Tuple<int, int>(id, 1);
                            MaxPriorityQueue.InvertedIndex tempInvIndex = new MaxPriorityQueue.InvertedIndex(tf, tempTuple);
                            result.Add(tempInvIndex);
                        }
                    }

                    return result;

                case 2:

                    temp = tier2.Get(term);
                    if (temp == null)
                    {
                        return new List<MaxPriorityQueue.InvertedIndex>();
                    }
                    else
                    {
                        foreach (MaxPriorityQueue.InvertedIndex index in temp)
                        {

                            int tf = index.GetTermFreq();
                            int id = index.GetDocumentId();
                            Tuple<int, int> tempTuple = new Tuple<int, int>(id, 2);
                            MaxPriorityQueue.InvertedIndex tempInvIndex = new MaxPriorityQueue.InvertedIndex(tf, tempTuple);
                            result.Add(tempInvIndex);

                        }
                    }

                    return result;

                case 3:

                    temp = tier3.Get(term);

                    if (temp == null)
                    {
                        return new List<MaxPriorityQueue.InvertedIndex>();
                    }
                    {
                        foreach (var index in temp)
                        {
                            int tf = index.GetTermFreq();
                            int id = index.GetDocumentId();
                            Tuple<int, int> tempTuple = new Tuple<int, int>(id, 3);
                            MaxPriorityQueue.InvertedIndex tempInvIndex = new MaxPriorityQueue.InvertedIndex(tf, tempTuple);
                            result.Add(tempInvIndex);
                        }
                    }

                    return result;
                default:
                    //an empty posting if the term does not exist.
                    return result;
            }

        }
        public IReadOnlyList<string> GetVocabulary()
        {
            return regIndex.GetVocabulary();
        }


        public DiskPositionalIndex.PostingDocWeight GetPostingDocWeight(int docID)
        {
            DiskPositionalIndex.PostingDocWeight result = regIndex.GetPostingDocWeight(docID);
            //Console.WriteLineConsole.WriteLine("Doc Weight: " + result.GetDocWeight() + "for document " + docID);
            return result;
        }

        public IList<Posting> GetPostings(string term)
        {
            return regIndex.GetPostings(term);
        }

        /// <summary>
        /// Retrieves a list of Postings of documents that contain the given list of terms.
        /// This returns postings only with docIDs
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        public IList<Posting> GetPostings(List<string> term)
        {
            return regIndex.GetPostings(term);
        }

        /// <summary>
        /// Retrieves a list of Postings of documents that contain the given term.
        /// This returns postings with docID and positions.
        /// </summary>
        /// <param name="term">a processed string</param>
        public IList<Posting> GetPositionalPostings(string term)
        {
            return regIndex.GetPositionalPostings(term);
        }

        /// <summary>
        /// Retrieves a list of Postings of documents that contain the given list of terms.
        /// This returns postings with docID and positions.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        public IList<Posting> GetPositionalPostings(List<string> term)
        {
            return regIndex.GetPositionalPostings(term);
        }
    }

}
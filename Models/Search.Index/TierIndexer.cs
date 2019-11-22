using Search.Document;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Timers;


namespace Search.Index
{
    public class TierIndexer
    {
        //Given an in memory index
        //Generates the three indexes for the tiered index
        public static IIndex CreateTiers(DiskPositionalIndex index)
        {
            //Collects the vocabulary of the on memory index
            List<string> vocab = index.GetVocabulary();
            //creates a priority queue for the purposes
            //of ordering the documents in terms of how frequently the doc contains the term
            MaxPriorityQueue termQueue = new MaxPriorityQueue();

            //What are these lines about?
            //Tier1Hashmap
            //Tier2Hashmap
            //Tier3Hashmap
            //Tier 1
            //Tier 2
            //Tier 3

            //this foreach loop constructs the 3 indices
            //for each word in the vocab...
            foreach (string v in vocab)
            {
                //
                List<Posting> postings = index.GetPositionalPosting(v);
                //
                foreach (Posting p in postings)
                {
                    //
                    int tf = p.Postions.Count();
                    //
                    int docID = p.DocumentID;
                    //
                    termQueue.MaxHeapInsert(tf, docID);
                }
                //
                List<MaxPriorityQueue.InvertedIndex> Tier1 = termQueue.RetrieveTier(1);
                //
                List<MaxPriorityQueue.InvertedIndex> Tier2 = termQueue.RetrieveTier(10);
                //
                List<MaxPriorityQueue.InvertedIndex> Tier3 = termQueue.GetPriorityQueue();
                //
                foreach ()
                {
                    //
                    foreach (MaxPriorityQueue.InvertedIndex t in Tier1)
                    {
                        //
                        int docID = t.GetDocumentId();
                        //foreach(int p in positions)
                        //{
                        //replace with replace
                        //Tier1Hashmap.AddTerm(v, docID, p);
                        //}
                    }
                }
                //
                termQueue.Clear();
            }

        }
    }
}



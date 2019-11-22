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
            IReadOnlyList<string> vocab = index.GetVocabulary();
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
                IList<Posting> postings = index.GetPositionalPostings(v);
                foreach (Posting p in postings)
                {
                    int tf = p.Positions.Count();
                    int docID = p.DocumentId;
                    termQueue.MaxHeapInsert(tf, docID);
                }
                //
                List<MaxPriorityQueue.InvertedIndex> Tier1 = termQueue.RetrieveTier(1);
                //
                List<MaxPriorityQueue.InvertedIndex> Tier2 = termQueue.RetrieveTier(10);
                //
                List<MaxPriorityQueue.InvertedIndex> Tier3 = termQueue.GetPriorityQueue();
           
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
                termQueue.ClearHeap();
            }

        }
    }
}



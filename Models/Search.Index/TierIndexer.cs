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
        //Given an in memory index and a path
        //Generates the three indices for the tiered index
        public static void CreateTiers(DiskPositionalIndex index, string path)
        {
            //Collects the vocabulary of the on memory index
            IReadOnlyList<string> vocab = index.GetVocabulary();
 
            //creates a priority queue for the purposes
            //of ordering the documents in terms of how frequently the doc contains the term
            MaxPriorityQueue termQueue = new MaxPriorityQueue();

            //declares the diskpositional indices  
            DiskPositionalIndex Tier1Hashmap = new DiskPositionalIndex(path);
            DiskPositionalIndex Tier2Hashmap = new DiskPositionalIndex(path);
            DiskPositionalIndex Tier3Hashmap = new DiskPositionalIndex(path);
            
            //hashmap of docIds and postings used for AddTerm function
            Dictionary<int, List<int>> docIDsAndPostings = new Dictionary<int, List<int>>();

            //this foreach loop constructs the 3 indices
            //for each word in the vocab...
            foreach (string v in vocab)
            {
                //get postings for the term
                IList<Posting> postings = index.GetPositionalPostings(v);
                
                //for each posting, get the term frequency and docID
                //to populate the MaxHeap and docId/posting hashmap
                foreach (Posting p in postings)
                {
                    int tf = p.Positions.Count;
                    int docID = p.DocumentId;
                    docIDsAndPostings.Add(docID,p.Positions);
                    termQueue.MaxHeapInsert(tf, docID);
                }

                //create 3 lists of docIDs 
                List<MaxPriorityQueue.InvertedIndex> Tier1 = termQueue.RetrieveTier(1);
                List<MaxPriorityQueue.InvertedIndex> Tier2 = termQueue.RetrieveTier(10);
                List<MaxPriorityQueue.InvertedIndex> Tier3 = termQueue.GetPriorityQueue();

                //clear the MaxHeap
                termQueue.ClearHeap();

                foreach (MaxPriorityQueue.InvertedIndex t in Tier1)
                {
                    int docID = t.GetDocumentId();
                    List<int> positionList = docIDsAndPostings[docID];
                    foreach (int p in positionList){
                    Tier1Hashmap.AddTerm(v, docID, p);
                    }
                }

                foreach (MaxPriorityQueue.InvertedIndex t in Tier2)
                {
                    int docID = t.GetDocumentId();
                    List<int> positionList = docIDsAndPostings[docID];
                    foreach (int p in positionList){
                    Tier2Hashmap.AddTerm(v, docID, p);
                    }
                }

                foreach (MaxPriorityQueue.InvertedIndex t in Tier3)
                {
                    int docID = t.GetDocumentId();
                    List<int> positionList = docIDsAndPostings[docID];
                    foreach (int p in positionList){
                    Tier3Hashmap.AddTerm(v, docID, p);
                    }
                }

                //clear hashmap of docIDs/postings for later use
                docIDsAndPostings.Clear();
            }

            //save the indices to the Disk
            Tier1Hashmap.Save();
            Tier2Hashmap.Save();
            Tier3Hashmap.Save();

        }
    }
}
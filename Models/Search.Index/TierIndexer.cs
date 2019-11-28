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
        public static void CreateTierIndices(DiskPositionalIndex index)
        {
            //Collects the vocabulary of the on memory index
            IReadOnlyList<string> vocab = index.GetVocabulary();

            //creates a priority queue for the purposes
            //of ordering the documents in terms of how frequently the doc contains the term
            MaxPriorityQueue termQueue = new MaxPriorityQueue();

            //Generate directory if we need to index corpus.
            Directory.CreateDirectory(Path.Join(Indexer.path, "/TierIndex1/"));
            //Generate directory if we need to index corpus.
            Directory.CreateDirectory(Path.Join(Indexer.path, "/TierIndex2/"));
            //Generate directory if we need to index corpus.
            Directory.CreateDirectory(Path.Join(Indexer.path, "/TierIndex3/"));

            //declares the diskpositional indices  
            DiskPositionalIndex Tier1Hashmap = new DiskPositionalIndex(Indexer.path+"/TierIndex1/");
            DiskPositionalIndex Tier2Hashmap = new DiskPositionalIndex(Indexer.path+"/TierIndex2/");
            DiskPositionalIndex Tier3Hashmap = new DiskPositionalIndex(Indexer.path+"/TierIndex3/");

            //hashmap of docIds and postings used for AddTerm function
            Dictionary<int, List<int>> docIDsAndPostings = new Dictionary<int, List<int>>();

            //this foreach loop constructs the 3 indices
            //for each word in the vocab...
            foreach (string term in vocab)
            {
                //get postings for the term
                IList<Posting> postings = index.GetPositionalPostings(term);

                //for each posting, get the term frequency and docID
                //to populate the MaxHeap and docId/posting hashmap
                foreach (Posting p in postings)
                {
                    int tf = p.Positions.Count;
                    int docID = p.DocumentId;
                    docIDsAndPostings.Add(docID, p.Positions);
                    termQueue.MaxHeapInsert(tf, docID);
                }

                //create 3 lists of docIDs 
                List<MaxPriorityQueue.InvertedIndex> Tier1 = termQueue.RetrieveTier(1);
                List<MaxPriorityQueue.InvertedIndex> Tier2 = termQueue.RetrieveTier(10);
                List<MaxPriorityQueue.InvertedIndex> Tier3 = termQueue.GetPriorityQueue();



                //adds the document to its proper tier 
                TierIndexer.BuildTierIndex(Tier1, docIDsAndPostings, term, Tier1Hashmap);
                TierIndexer.BuildTierIndex(Tier2, docIDsAndPostings, term, Tier2Hashmap);
                TierIndexer.BuildTierIndex(Tier3, docIDsAndPostings, term, Tier3Hashmap);

                //clear hashmap of docIDs/postings for later use
                docIDsAndPostings.Clear();

                //clear the MaxHeap
                termQueue.ClearHeap();
            }

            //save the indices to the Disk
            Tier1Hashmap.SaveTier();
            Tier2Hashmap.SaveTier();
            Tier3Hashmap.SaveTier();

        }

        private static void BuildTierIndex(List<MaxPriorityQueue.InvertedIndex> tier, Dictionary<int, List<int>> docIDsAndPostings, string term, DiskPositionalIndex TierHashMap)
        {

            foreach (MaxPriorityQueue.InvertedIndex t in tier)
            {
                int docID = t.GetDocumentId();
                List<int> positionList = docIDsAndPostings[docID];
                foreach (int p in positionList)
                {
                    TierHashMap.AddTerm(term, docID, p);
                }
            }
        }
    }
}
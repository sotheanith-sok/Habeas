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
        public static void CreateTierIndices( Dictionary<string, List<Posting>> index)
        {
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();
            
            //Collects the vocabulary of the on memory index
            IReadOnlyList<string> vocab = index.Keys.ToList();
            Console.WriteLine("Got Vocab: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

            //creates a priority queue for the purposes
            //of ordering the documents in terms of how frequently the doc contains the term
            MaxPriorityQueue termQueue = new MaxPriorityQueue();

            //Generate directory if we need to index corpus.
            Directory.CreateDirectory(Path.Join(Indexer.path, "TierIndex1/"));
            //Generate directory if we need to index corpus.
            Directory.CreateDirectory(Path.Join(Indexer.path, "TierIndex2/"));
            //Generate directory if we need to index corpus.
            Directory.CreateDirectory(Path.Join(Indexer.path, "TierIndex3/"));
            Console.WriteLine("Built MPQ and Directories: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

            //declares the diskpositional indices  
            DiskPositionalIndex Tier1Hashmap = new DiskPositionalIndex(Indexer.path+"TierIndex1/");
            Tier1Hashmap.Clear();
            DiskPositionalIndex Tier2Hashmap = new DiskPositionalIndex(Indexer.path+"TierIndex2/");
            Tier2Hashmap.Clear();
            DiskPositionalIndex Tier3Hashmap = new DiskPositionalIndex(Indexer.path+"TierIndex3/");
            Tier3Hashmap.Clear();
            Console.WriteLine("Declared DPI " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

            //hashmap of docIds and postings used for AddTerm function
            Dictionary<int, List<int>> docIDsAndPostings = new Dictionary<int, List<int>>();

            //this foreach loop constructs the 3 indices
            //for each word in the vocab...
            long i = 0;
            foreach (string term in vocab)
            {
                i++;
                Console.WriteLine("#"+i+". Dealing with vocab term "+ term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
                //get postings for the term
                IList<Posting> postings = index[term];
                Console.WriteLine("Got "+postings.Count+" postings for "+term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

                //for each posting, get the term frequency and docID
                //to populate the MaxHeap and docId/posting hashmap
                foreach (Posting p in postings)
                {
                    //Console.WriteLine("start: "+elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
                    int tf = p.Positions.Count;
                    //Console.WriteLine("count "+ tf+" at: "+elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
                    int docID = p.DocumentId;
                    //Console.WriteLine("ID "+docID+": "+elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
                    docIDsAndPostings.Add(docID, p.Positions);
                    //Console.WriteLine("Posting Add: "+elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
                    termQueue.MaxHeapInsert(tf, docID);
                    //Console.WriteLine("Insert Add: "+elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
                }
                //ths process s tme consumng
                Console.WriteLine("Inserted postings for "+term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

                //create 3 lists of docIDs 
                List<MaxPriorityQueue.InvertedIndex> Tier1 = termQueue.RetrieveTier(1);
                List<MaxPriorityQueue.InvertedIndex> Tier2 = termQueue.RetrieveTier(10);
                List<MaxPriorityQueue.InvertedIndex> Tier3 = termQueue.RetrieveTier(100);
                //
                Console.WriteLine("Created Lists for "+term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

                //adds the document to its proper tier 
                TierIndexer.BuildTierIndex(Tier1, docIDsAndPostings, term, Tier1Hashmap);
                TierIndexer.BuildTierIndex(Tier2, docIDsAndPostings, term, Tier2Hashmap);
                TierIndexer.BuildTierIndex(Tier3, docIDsAndPostings, term, Tier3Hashmap);
                Console.WriteLine("Created tier for "+term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

                //clear hashmap of docIDs/postings for later use
                docIDsAndPostings.Clear();
                
                Console.WriteLine("Cleared docsAndPostings for "+term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

                //clear the MaxHeap
                termQueue.ClearHeap();
                Console.WriteLine("Cleared Heap for "+term+ " at: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
            }

            
            //save the indices to the Disk
            Tier1Hashmap.SaveTier();
            Tier2Hashmap.SaveTier();
            Tier3Hashmap.SaveTier();

            elapsedTime.Stop();
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
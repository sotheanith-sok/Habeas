// using Search.Document;
// using Search.Text;
// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.IO;
// using System.Timers;


// namespace Search.Index
// {
//     public class TierIndexer
//     {

//         public static int counter = 0;
//         //Given an in memory index and a path
//         //Generates the three indices for the tiered index
//         public static void CreateTierIndices(Dictionary<string, List<Posting>> tempPostingMap)
//         {
//             Console.WriteLine("[Tier Indexer]: Creating Tier Indices");

//             List<Posting> termPostings;
//             List<string> vocab = tempPostingMap.Keys.ToList();
            

//             //Generate directory if we need to index corpus.
//             Directory.CreateDirectory(Path.Join(Indexer.path, "TierIndex1/"));
//             //Generate directory if we need to index corpus.
//             Directory.CreateDirectory(Path.Join(Indexer.path, "TierIndex2/"));
//             //Generate directory if we need to index corpus.
//             Directory.CreateDirectory(Path.Join(Indexer.path, "TierIndex3/"));
//             Console.WriteLine("Built MPQ and Directories: " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

//             //declares the diskpositional indices  
//             DiskPositionalIndex Tier1Hashmap = new DiskPositionalIndex(Indexer.path + "TierIndex1/");
//             Tier1Hashmap.Clear();
//             DiskPositionalIndex Tier2Hashmap = new DiskPositionalIndex(Indexer.path + "TierIndex2/");
//             Tier2Hashmap.Clear();
//             DiskPositionalIndex Tier3Hashmap = new DiskPositionalIndex(Indexer.path + "TierIndex3/");
//             Tier3Hashmap.Clear();
//             Console.WriteLine("Declared DPI " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));


//             //Creates three postings list to each term and a posting list.
//             List<Posting> Tier1Postings = new List<Posting>();
//             List<Posting> Tier2Postings = new List<Posting>();
//             List<Posting> Tier3Postings = new List<Posting>();

//             int termPostingLength = new int();
//             //this foreach loop constructs the 3 indices
//             //for each word in the vocab...
//             long i = 0;
//             foreach (string term in vocab)
//             {

//                 //get postings for the term
//                 termPostings = tempPostingMap[term];

//                 //check the size of the posting list 
//                 termPostingLength = termPostings.Count;

//                 //sort this.termPostings according to highest term frequency
//                 termPostings.Sort(
//                     delegate (Posting post1, Posting post2)
//                     {
//                         return post2.Positions.Count.CompareTo(post1.Positions.Count);
//                     }
//                 );

//                 Tier1Postings = RetrieveTopPercent(1, term, termPostingLength, termPostings);
//                 Tier1Postings.Sort(
//                     delegate (Posting post1, Posting post2)
//                     {
//                         return post1.DocumentId.CompareTo(post2.DocumentId);
//                     });
//                 if (termPostings.Count > 0)
//                 {
//                     Tier2Postings = RetrieveTopPercent(10, term, termPostingLength, termPostings);
//                     Tier2Postings.Sort(
//                     delegate (Posting post1, Posting post2)
//                     {
//                         return post1.DocumentId.CompareTo(post2.DocumentId);
//                     });
//                 }

//                 if (termPostings.Count > 0)
//                 {
//                     Tier3Postings.AddRange(termPostings);
//                     Tier3Postings.Sort(
//                    delegate (Posting post1, Posting post2)
//                    {
//                        return post1.DocumentId.CompareTo(post2.DocumentId);
//                    });
//                 }
//                 //Add terms to temporary posting map in DiskPositionalIndex
//                 Tier1Hashmap.SetTempPostingMap(term, Tier1Postings);
//                 if (termPostings.Count > 0)
//                 {
//                     Tier2Hashmap.SetTempPostingMap(term, Tier2Postings);
//                 }
//                 if (termPostings.Count > 0)
//                 {
//                     Tier3Hashmap.SetTempPostingMap(term, Tier3Postings);
//                 }
//                 TierIndexer.counter =0;
//             }



//             //save the indices to the Disk
//             Tier1Hashmap.SaveTier();

//             Tier2Hashmap.SaveTier();
//             Tier3Hashmap.SaveTier();



//             //Clear Postings
//             Tier1Postings.Clear();
//             Tier2Postings.Clear();
//             Tier3Postings.Clear();
//             Console.WriteLine(vocab.Count);
//             Console.WriteLine("[Tier Indexer] Finished Creating Tier Indices");
//         } //end Create Tier Method


//         public static List<Posting> RetrieveTopPercent(double percentOfDocuments, string term, int termPostingsLength, List<Posting> termPostings)
//         {


//             //temproary variable for a posting
//             Posting posting;
//             List<Posting> tempPosting = new List<Posting>();

//             //calculate the limit for each tier
//             double limit = Math.Floor((percentOfDocuments * termPostingsLength) / 100);

//             //if posting size is small then just fit into tier 1... most likely we are dealing with a small corpus?
//             if (limit <= 1)
//             {
//                 tempPosting.AddRange(termPostings);
//                 termPostings.Clear();

//                 return tempPosting;
//             }
//             else
//             {
//                 while (tempPosting.Count < limit)
//                 {
//                     if (termPostings.Count == 0)
//                     {
//                         break;
//                     }
//                     else
//                     {
//                         posting = termPostings[TierIndexer.counter];
//                         TierIndexer.counter = TierIndexer.counter+1;
//                         Console.WriteLine(TierIndexer.counter);
//                         tempPosting.Add(posting);
//                         //termPostings.Remove(posting);
//                     }
//                 }
//             }
//             Console.WriteLine(TierIndexer.counter = TierIndexer.counter + 1);

//             return tempPosting;


//         }//end RetriveTopPErcent

//     }// end Tier Indexer Class

// }//end namepsace

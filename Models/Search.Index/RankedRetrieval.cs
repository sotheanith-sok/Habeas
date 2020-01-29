using System.Collections.Generic;
using Search.Document;
using System;
using System.Diagnostics;


namespace Search.Index
{
    public class RankedRetrieval
    {
        public int resultsReturned = 50;
        //w_{q,t}
        private double query2termWeight;
        //w_{d,t}
        private double doc2termWeight;

        //temporarily stores the document id with its corresponding rank  [docId -> A_{docID}]
        private Dictionary<int, double> accumulator;

        public List<int> NonZeroAccumulatorCounts { get; }

        //used to calculate the queryToTermWeight
        private int corpusSize;


        //saves instance of the corpus to access corpus path
        private IDocumentCorpus corpus;

        private List<int> documentIds;


        IIndex index;

        IRankVariant rankVariant;

        //Make a new priority queue
        MaxPriorityQueue FinalResultPriorityQueue = new MaxPriorityQueue();
        public RankedRetrieval(IDocumentCorpus corpus, IIndex index, string RankedRetrievalMode)
        {
            query2termWeight = new int();
            doc2termWeight = new int();
            accumulator = new Dictionary<int, double>();
            documentIds = new List<int>();

            this.rankVariant = SetRankedRetrievalMode(RankedRetrievalMode);
            this.index = index;
            this.corpus = corpus;

            string path = Indexer.path;
            this.corpusSize = this.GetCorpusSize(path);

            NonZeroAccumulatorCounts = new List<int>();
        }

        /// <summary>
        /// Count the number of documents from index
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int GetCorpusSize(string path)
        {
            return corpus.CorpusSize;
        }


        public IRankVariant SetRankedRetrievalMode(string RankedRetrievalMode)
        {
            switch (RankedRetrievalMode)
            {
                case "Tf-idf":
                    return new Tf_Idf();
                case "Okapi":
                    return new Okapi();

                case "Wacky":
                    return new Wacky();
                default:
                    return new Default();
            }

        }



        /// <summary>
        /// Method that takes in the query and returns a list of the top ten ranking documents
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// 

        public IList<MaxPriorityQueue.InvertedIndex> GetTopTen(List<string> query)
        {

            this.FinalResultPriorityQueue = BuildAccumulatorQueue(query);

            //WARN: temporary to get the NonZeroAccumulatorCounts
            NonZeroAccumulatorCounts.Add(accumulator.Count);

            //clear accumulator for next search
            accumulator.Clear();

            //Retrieve Top Ten Documents according to percent
            return this.FinalResultPriorityQueue.RetrieveTopTen();

        }

        public IList<MaxPriorityQueue.InvertedIndex> GetTopTenTier(List<string> query)
        {

            // //Build the Accumulator Hashmap
            // //Build Priority Queue using the Accumulator divided by L_{d}  



            //For Tiered Indices
            this.FinalResultPriorityQueue = BuildAccumulatorTierQueue(query);
            if (this.FinalResultPriorityQueue.GetPriorityQueue().Count < 50 && query.Count > 1)
            {
                BuildAccumulatorTierQueue(query, 2);
            }
            if (this.FinalResultPriorityQueue.GetPriorityQueue().Count < 50 && query.Count > 1)
            {
                BuildAccumulatorTierQueue(query, 3);
            }

            //WARN: temporary to get the NonZeroAccumulatorCounts
            NonZeroAccumulatorCounts.Add(accumulator.Count);

            accumulator.Clear();



            //Retrieve Top Ten Documents according to percent
            return this.FinalResultPriorityQueue.RetrieveTopTen();

        }

        private MaxPriorityQueue BuildAccumulatorTierQueue(List<string> query, int tier = 1)
        {
            // Stopwatch stopwatch = new Stopwatch();
            // stopwatch.Start();

            this.FinalResultPriorityQueue.ClearHeap();

            Dictionary<int, int> id2tier = new Dictionary<int, int>();
            //stores temporary Accumulator value that will be added to the accumulator hashmap
            double docAccumulator;
            List<MaxPriorityQueue.InvertedIndex> docIDS = new List<MaxPriorityQueue.InvertedIndex>();

            foreach (string term in query)
            {
                if (tier == 1) // get postings only from tier
                {
                    docIDS = this.index.GetPostingsFromTier(term);
                }
                else if (tier == 2) //get postings from tier 1 and tier 2
                {
                    for (int i = 1; i < tier; i++)
                    {
                        docIDS.AddRange(this.index.GetPostingsFromTier(term, i));
                    }
                }
                else //get postings from all tiers
                {
                    for (int i = 1; i < tier; i++)
                    {
                        docIDS.AddRange(this.index.GetPostingsFromTier(term, i));
                    }

                }

                //documentFrequency
                int docFrequency = docIDS.Count;

                //implements formula for w_{q,t}
                this.query2termWeight = this.rankVariant.calculateQuery2TermWeight(docFrequency, this.corpusSize);

                foreach (MaxPriorityQueue.InvertedIndex item in docIDS)
                {
                    double termFrequency = item.GetTermFreq();
                    int docID = item.GetTuple().Item1;
                    int tierID = item.GetTuple().Item2;


                    if (!id2tier.ContainsKey(docID))
                    {
                        id2tier.Add(docID, tierID);
                    }


                    //implements formula for w_{d,t}
                    this.doc2termWeight = this.rankVariant.calculateDoc2TermWeight(termFrequency, docID, this.corpusSize, this.index);

                    //the A_{d} value for a specific term in that document
                    docAccumulator = this.query2termWeight * this.doc2termWeight;

                    //if the A_{d} value exists on the hashmap increase its value else create a new key-value pair
                    if (accumulator.ContainsKey(docID))
                    {
                        this.accumulator[docID] += docAccumulator;
                    }
                    else
                    {
                        this.accumulator.Add(docID, docAccumulator);
                    }
                }
            }

            //Console.WriteLine("After initial for each: " + stopwatch.ElapsedMilliseconds);
            //temporary variable to hold the doc weight
            double normalizer;
            //temporary variable to hold the final ranking value of that document
            double finalRank;

            //Console.WriteLine("Rank returned by the Accumulator: ");
            //for every key value in the Accumulator divide A_{d} by L_{d}
            foreach (KeyValuePair<int, double> candidate in this.accumulator)
            {
                //get corresponding L_{d} value according to ranking system
                normalizer = this.rankVariant.GetDocumentWeight(candidate.Key, this.index);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = (double)candidate.Value / normalizer;

                int tierValue = id2tier[candidate.Key];
                //Console.WriteLine("Candidate " + candidate + " Rank Score" + finalRank + " ");
                Tuple<int, int> tempTuple = new Tuple<int, int>(candidate.Key, tierValue);

                //add to list to perform priority queue on 
                this.FinalResultPriorityQueue.MaxHeapInsert(finalRank, tempTuple);
            }
            //Console.WriteLine("End of Ranking returned by the Accumulator");

            // stopwatch.Stop();
            //Console.WriteLine("Elapsed time for query: " + stopwatch.ElapsedMilliseconds);
            return this.FinalResultPriorityQueue;

        } // end of BuildAccumulatorQueue()

        /// <summary>
        /// Builds the Accumulator hashmap for the query to retrieve top 10 documents
        /// </summary>
        /// <param name="query"></param>
        private MaxPriorityQueue BuildAccumulatorQueue(List<string> query)
        {
            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();

            //stores temporary Accumulator value that will be added to the accumulator hashmap
            double docAccumulator;


            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {
                //posting list of a term grabbed from the On Disk file
                IList<Posting> docIDS = this.index.GetPostings(term);

                //documentFrequency
                int docFrequency = docIDS.Count;

                //implements formula for w_{q,t}
                this.query2termWeight = this.rankVariant.calculateQuery2TermWeight(docFrequency, this.corpusSize);

                foreach (Posting item in docIDS)
                {
                    double termFrequency = item.Positions.Count;
                    int docID = item.DocumentId;


                    //implements formula for w_{d,t}
                    this.doc2termWeight = this.rankVariant.calculateDoc2TermWeight(termFrequency, docID, this.corpusSize, this.index);

                    //the A_{d} value for a specific term in that document
                    docAccumulator = this.query2termWeight * this.doc2termWeight;

                    //if the A_{d} value exists on the hashmap increase its value else create a new key-value pair
                    if (accumulator.ContainsKey(docID))
                    {
                        this.accumulator[docID] += docAccumulator;
                    }
                    else
                    {
                        this.accumulator.Add(docID, docAccumulator);
                    }
                } // end of foreach loop for postings in docIDS

            } // end of foreach loop for query

            //Console.WriteLine("After initial for each: " + stopwatch.ElapsedMilliseconds);

            //temporary variable to hold the doc weight
            double normalizer;
            //temporary variable to hold the final ranking value of that document
            double finalRank;

            //Make a new priority queue
            MaxPriorityQueue rankQueue = new MaxPriorityQueue();

            //Console.WriteLine("Rank returned by the Accumulator: ");
            //for every key value in the Accumulator divide A_{d} by L_{d}
            foreach (KeyValuePair<int, double> candidate in this.accumulator)
            {
                //get corresponding L_{d} value according to ranking system
                normalizer = this.rankVariant.GetDocumentWeight(candidate.Key, this.index);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = (double)candidate.Value / normalizer;

                //Console.WriteLine("Candidate " + candidate + " Rank Score" + finalRank + " ");


                //add to list to perform priority queue on 
                rankQueue.MaxHeapInsert(finalRank, candidate.Key);
            }
            //Console.WriteLine("End of Ranking returned by the Accumulator");

            //stopwatch.Stop();
            // Console.WriteLine("Elapsed time for query: " + stopwatch.ElapsedMilliseconds);

            return rankQueue;

        } // end of BuildPriorityQueuer();

    }

}


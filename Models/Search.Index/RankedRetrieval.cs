using System.Collections.Generic;
using Search.Document;


namespace Search.Index
{
    public class RankedRetrieval
    {
        //w_{q,t}
        private double query2termWeight;
        //w_{d,t}
        private double doc2termWeight;

        //temporarily stores the document id with its corresponding rank  [docId -> A_{docID}]
        private Dictionary<int, double> accumulator;

        //used to calculate the queryToTermWeight
        private int corpusSize;


        //saves instance of the corpus to access corpus path
        private IDocumentCorpus corpus;

        private List<int> documentIds;


        IIndex index;

        IRankVariant rankVariant;

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
        public IList<MaxPriorityQueue.InvertedIndex> GetTopTen(List<string> query)
        {

            // //Build the Accumulator Hashmap
            // BuildAccumulator(query);

            // //Build Priority Queue using the Accumulator divided by L_{d}  
            // MaxPriorityQueue priorityQueue = BuildPriorityQueue();

            MaxPriorityQueue priorityQueue = BuildAccumulatorQueue(query);

            accumulator.Clear();

            //Retrieve Top Ten Documents according to percent
            return priorityQueue.RetrieveTopTen();

        }

        private MaxPriorityQueue BuildAccumulatorQueue(List<string> query)
        {
            //stores temporary Accumulator value that will be added to the accumulator hashmap
            double docAccumulator;
            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {
                //posting list of a term grabbed from the On Disk file
                List<MaxPriorityQueue.InvertedIndex> docIDS = this.index.GetPostingsFromTier(term);

                //documentFrequency
                int docFrequency = docIDS.Count;

                //implements formula for w_{q,t}
                this.query2termWeight = this.rankVariant.calculateQuery2TermWeight(docFrequency, this.corpusSize);

                foreach (MaxPriorityQueue.InvertedIndex item in docIDS)
                {
                    double termFrequency = item.GetTermFreq();
                    int docID = item.GetDocumentId();


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




            } //end of for each

            //temporary variable to hold the doc weight
            double normalizer;
            //temporary variable to hold the final ranking value of that document
            double finalRank;

            //Make a new priority queue
            MaxPriorityQueue priorityQueue = new MaxPriorityQueue();

            
            //for every key value in the Accumulator divide A_{d} by L_{d}
            foreach (KeyValuePair<int, double> candidate in this.accumulator)
            {
                //get corresponding L_{d} value according to ranking system
                normalizer = this.rankVariant.GetDocumentWeight(candidate.Key, this.index);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = (double)candidate.Value / normalizer;

                //add to list to perform priority queue on 
                priorityQueue.MaxHeapInsert(finalRank, candidate.Key);
            }

            return priorityQueue;

        } // end of BuildAccumulatorQueue()

        /// <summary>
        /// Builds the Accumulator hashmap for the query to retrieve top 10 documents
        /// </summary>
        /// <param name="query"></param>
        private void BuildAccumulator(List<string> query)
        {

            //stores temporary Accumulator value that will be added to the accumulator hashmap
            double docAccumulator;


            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {
                //posting list of a term grabbed from the On Disk file
                List<MaxPriorityQueue.InvertedIndex> docIDS = this.index.GetPostingsFromTier(term);

                //documentFrequency
                int docFrequency = docIDS.Count;

                //implements formula for w_{q,t}
                this.query2termWeight = this.rankVariant.calculateQuery2TermWeight(docFrequency, this.corpusSize);

                foreach (MaxPriorityQueue.InvertedIndex item in docIDS)
                {
                    double termFrequency = item.GetTermFreq();
                    int docID = item.GetDocumentId();


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



        } // end of BuildAccumulator(List<String> query)

        /// <summary>
        /// Creates a new priority queue by inserting the rank of the document and document id 
        /// </summary>
        /// <returns> a priority queue with max heap property</returns>
        private MaxPriorityQueue BuildPriorityQueue()
        {

            //temporary variable to hold the doc weight
            double normalizer;
            //temporary variable to hold the final ranking value of that document
            double finalRank;

            //Make a new priority queue
            MaxPriorityQueue priorityQueue = new MaxPriorityQueue();


            //for every key value in the Accumulator divide A_{d} by L_{d}
            foreach (KeyValuePair<int, double> candidate in this.accumulator)
            {
                //get corresponding L_{d} value according to ranking system
                normalizer = this.rankVariant.GetDocumentWeight(candidate.Key, this.index);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = (double)candidate.Value / normalizer;

                //add to list to perform priority queue on 
                priorityQueue.MaxHeapInsert(finalRank, candidate.Key);
            }

            return priorityQueue;

        } // end of BuildPriorityQueuer();

    }

}


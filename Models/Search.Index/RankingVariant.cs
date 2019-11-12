using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using Search.Text;
using Search.Document;
using Search.Query;

namespace Search.Index
{
    public class RankingVariant
    {
        //w_{q,t}
        private double query2termWeight;
        //w_{d,t}
        private double doc2termWeight;

        //temporarily stores the document id with its corresponding rank  [docId -> A_{docID}]
        private Dictionary<int, double> accumulator;

        private int corpusSize;

        private double averageDocLength;

        private IDocumentCorpus corpus;

        public RankingVariant(IDocumentCorpus corpus)
        {
            query2termWeight = new int();
            doc2termWeight = new int();
            accumulator = new Dictionary<int, double>();

            averageDocLength = new double();
            this.corpus = corpus;
        }


        /// <summary>
        /// Method that takes in the query and returns a list of the top ten ranking documents
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList<MaxPriorityQueue.InvertedIndex> GetRankedDocuments(IIndex index, List<string> query, string RankedRetrievalMode)
        {

            //Build the Accumulator Hashmap
            BuildAccumulator(query, RankedRetrievalMode, index);

            //Build Priority Queue using the Accumulator divided by L_{d}  
            MaxPriorityQueue pq = BuildPriorityQueue(RankedRetrievalMode);

            accumulator.Clear();

            //Retrieve Top Ten Documents and Return to Back End
            return pq.RetrieveTopTen();

        }


        /// <summary>
        /// Builds the Accumulator hashmap for the query to retrieve top 10 documents
        /// </summary>
        /// <param name="query"></param>
        private void BuildAccumulator(List<string> query, string RankedRetrievalMode, IIndex index)
        {

            //stores temporary Accumulator value that will be added to the accumulator hashmap
            double docAccumulator;
            //gets path to access on disk file
            string path = Indexer.path;

            //getCorpusSize
            this.corpusSize = this.GetCorpusSize(path);



            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {
                //posting list of a term grabbed from the On Disk file
                IList<Posting> postings = index.GetPostings(term);


                if (postings != default(List<Posting>))
                {
                    int docFrequency = postings.Count;

                    //implements formula for w_{q,t}
                    this.query2termWeight = calculateQuery2TermWeight(docFrequency, RankedRetrievalMode);

                    foreach (Posting post in postings)
                    {
                        int termFrequency = post.Positions.Count;


                        //implements formula for w_{d,t}
                        this.doc2termWeight = calculateDoc2TermWeight(termFrequency, RankedRetrievalMode, post.DocumentId, index);

                        //the A_{d} value for a specific term in that document
                        docAccumulator = this.query2termWeight * this.doc2termWeight;

                        //if the A_{d} value exists on the hashmap increase its value else create a new key-value pair
                        if (accumulator.ContainsKey(post.DocumentId))
                        {
                            this.accumulator[post.DocumentId] += docAccumulator;
                        }
                        else
                        {
                            this.accumulator.Add(post.DocumentId, docAccumulator);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new priority queue by inserting the rank of the document and document id 
        /// </summary>
        /// <returns> a priority queue with max heap property</returns>
        private MaxPriorityQueue BuildPriorityQueue(string RankedRetrievalMode)
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
                normalizer = GetDocumentWeight(candidate.Key, RankedRetrievalMode);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = (double)candidate.Value / normalizer;

                //add to list to perform priority queue on 
                priorityQueue.MaxHeapInsert(finalRank, candidate.Key);
            }

            return priorityQueue;

        }

        /// <summary>
        /// Calculates the correct W_{q,t} score depending on the ranking system the user desires to implement
        /// Case 1: td-idf
        /// Case 2: Okapi BM25
        /// Case 3: Wacky
        /// Default: Default Ranking
        /// </summary>
        /// <param name="docFrequency"></param>
        /// <param name="RankedRetrievalMode"></param>
        /// <returns></returns>
        public double calculateQuery2TermWeight(int docFrequency, string RankedRetrievalMode)
        {
            int N = this.corpusSize;
            switch (RankedRetrievalMode)
            {
                case "Tf-idf":
                    return Math.Log((double)N / docFrequency);
                case "Okapi":
                    double OkapiWqtValue = Math.Log((double)(N - docFrequency + 0.5) / (docFrequency + 0.5));
                    if (0.1 > OkapiWqtValue)
                    {
                        return 0.1;
                    }
                    else
                        return OkapiWqtValue;

                case "Wacky":
                    int numerator = N - docFrequency;
                    double division = (double)numerator / docFrequency;
                    if (division > 1)
                    {
                        double WackyWqtValue = Math.Log(division);
                        return WackyWqtValue;

                    }
                    else
                        return 0.0;


                default:
                    return Math.Log(1 + (double)N / docFrequency);
            }

        }

        /// <summary>
        /// Calculates the correct W_{d,t} score depending on the ranking system the user desires to implement
        /// Case 1: td-idf
        /// Case 2: Okapi BM25
        /// Case 3: Wacky
        /// Default : Default Ranking
        /// </summary>
        /// <param name="termFrequency"></param>
        /// <param name="RankedRetrievalMode"></param>
        /// <returns></returns>
        public double calculateDoc2TermWeight(int termFrequency, string RankedRetrievalMode, int docID, IIndex index)
        {
            int N = this.corpusSize;
            switch (RankedRetrievalMode)
            {
                case "Tf-idf":
                    return (double)termFrequency;

                case "Okapi":
                    int documentLength = Indexer.tokensPerDocument[docID];
                    calculateAverageDocLength();

                    double OkapiWdtValue = (double)(2.2 * termFrequency) / (1.2 * (0.25 + 0.75 * (double)documentLength / this.averageDocLength) + termFrequency);
                    return OkapiWdtValue;

                case "Wacky":
                    double avDocTermFreq = index.GetAverageTermFreq(docID);
                    double WackyWdtValue = (double)(1 + Math.Log(termFrequency)) / (1 + Math.Log(avDocTermFreq));
                    return WackyWdtValue;
                default:
                    return (double)(1 + Math.Log(termFrequency));
            }

        }

        /// <summary>
        /// Uses the document id to access the docWeights.bin file to retrieve the corresponding L_{d} value using the current ranking system
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        private double GetDocumentWeight(int docId, string RankedRetrievalMode)
        {
            string filePath = Indexer.path + "docWeights.bin";
            double docWeight;
            switch (RankedRetrievalMode)
            {
                case "Tf-idf":
                    using (BinaryReader docWeightsReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                    {
                        int startByte = docId * 8;

                        //Jump to the starting byte
                        docWeightsReader.BaseStream.Seek(startByte, SeekOrigin.Begin);
                        //Read a document weight and convert it
                        docWeight = BitConverter.Int64BitsToDouble(docWeightsReader.ReadInt64());
                    }
                    return docWeight;

                case "Okapi":
                    return 1.0;
                case "Wacky":
                    IDocument doc = this.corpus.GetDocument(docId);
                    string docFilePath = doc.FilePath;
                    int fileSizeInByte = (int)(new FileInfo(filePath).Length / 8f);
                    double WackyLd = (double)(Math.Sqrt(fileSizeInByte));
                    Console.WriteLine(WackyLd);
                    return WackyLd;

                default:
                    using (BinaryReader docWeightsReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                    {
                        int startByte = docId * 8;

                        //Jump to the starting byte
                        docWeightsReader.BaseStream.Seek(startByte, SeekOrigin.Begin);
                        //Read a document weight and convert it
                        docWeight = BitConverter.Int64BitsToDouble(docWeightsReader.ReadInt64());
                    }
                    return docWeight;
            }

        }

        /// <summary>
        /// Count the number of documents from index
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int GetCorpusSize(string path)
        {
            string filePath = Indexer.path + "docWeights.bin";
            return (int)(new FileInfo(filePath).Length / 8f);
        }

        /// <summary>
        /// calcuates the average token frequency of a particulat document
        /// </summary>
        private void calculateAverageDocLength()
        {
            double average = 0;
            foreach (KeyValuePair<int, int> docTokens in Indexer.tokensPerDocument)
            {
                average = average + docTokens.Value;
            }
            average = (double)average / Indexer.tokensPerDocument.Count;

            this.averageDocLength = average;
        }
    }

}


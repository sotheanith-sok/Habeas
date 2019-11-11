using System.Collections.Generic;
using System.Linq;
using Search.Query;
using System;
using Search.OnDiskDataStructure;
using System.IO;
namespace Search.Index
{
    public class DiskPositionalIndex : IIndex
    {
        //Hashmap is used to store the index. O(1)
        //Dictionary in C# is equivalent to HashMap in Java.
        private readonly SortedDictionary<string, List<Posting>> hashMap;

        //HashMap used to store termFrequency of current Document
        private readonly SortedDictionary<string, int> termFrequency;

        //maintains a list of the docWeights to store in the docWeights.bin file
        private static List<double> calculatedDocWeights;

        //maintains the hashmap for the posting list for a specific term
        private OnDiskDictionary<string, List<Posting>> onDiskPostingMap;

        //maintains a hashmap for the termfrequency for a specific term
        private OnDiskDictionary<string, int> onDiskTermFrequencyMap;

        //maintains a hashmap for the document weight for a specific document id
        private OnDiskDictionary<int, int> onDiskDocWeight;

        //temporarily stores the document id with its corresponding rank  [docId -> A_{docID}]
        private Dictionary<int, double> Accumulator;


        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public DiskPositionalIndex(string path)
        {
            hashMap = new SortedDictionary<string, List<Posting>>();
            termFrequency = new SortedDictionary<string, int>();
            calculatedDocWeights = new List<double>();

            onDiskPostingMap = new OnDiskDictionary<string, List<Posting>>(new StringEncoderDecoder(), new PostingListEncoderDecoder());
            onDiskTermFrequencyMap = new OnDiskDictionary<string, int>(new StringEncoderDecoder(), new IntEncoderDecoder());
            onDiskDocWeight = new OnDiskDictionary<int, int>(new IntEncoderDecoder(), new IntEncoderDecoder());

            Accumulator = new Dictionary<int, double>();
        }

        /// <summary>
        /// Gets Postings of a given term from in-memory index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {

            List<Posting> result = onDiskPostingMap.Get(term, Indexer.path, "Postings");
            if (default(List<Posting>) == result)
            {
                return new List<Posting>();
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Gets Postings of a given list of terms from in-memory index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {
            List<List<Posting>> postingLists = onDiskPostingMap.Get(terms, Indexer.path, "Postings");
            postingLists.RemoveAll(item => item == default(List<Posting>));
            return Merge.OrMerge(new List<IList<Posting>>(postingLists));
        }


        /// <summary>
        /// Gets Postings of a given term from in-memory index.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        public IList<Posting> GetPositionalPostings(string term)
        {
            return GetPostings(term);
        }

        /// <summary>
        /// Gets Postings of a given list of terms from in-memory index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPositionalPostings(List<string> terms)
        {
            return GetPostings(terms);
        }

        /// <summary>
        /// Gets a sorted list of all vocabularies from index.
        /// </summary>
        public IReadOnlyList<string> GetVocabulary()
        {
            List<string> vocabulary = onDiskPostingMap.GetKeys(Indexer.path, "Postings").ToList();
            vocabulary.Sort();
            return vocabulary;
        }


        /// <summary>
        /// Adds a term into the index with its docId and position.
        /// </summary>
        /// <param name="term">a processed string to be added</param>
        /// <param name="docID">the document id in which the term is coming from</param>
        /// <param name="position">the position of the term within the document</param>
        public void AddTerm(string term, int docID, int position)
        {
            //ChangeFrequency
            UpdateTermFrequencyForDoc(term);

            //Check if inverted index contains the term (key)
            if (hashMap.ContainsKey(term))
            {
                //Check if the document of the term is in the posting list
                Posting lastPosting = hashMap[term].Last();
                if (lastPosting.DocumentId == docID)
                {
                    //Add a position to the posting
                    lastPosting.Positions.Add(position);
                }
                else
                {
                    //Create a posting with (docID & position) to the posting list
                    hashMap[term].Add(new Posting(docID, new List<int> { position }));
                }

            }
            else
            {

                //Add term and a posting (docID & position) to the hashmap
                List<Posting> postingList = new List<Posting>();
                postingList.Add(new Posting(docID, new List<int> { position }));
                hashMap.Add(term, postingList);

            }

        }

        /// <summary>
        /// Increases the instance of a term in a document in our Term Frequence HashMap
        /// </summary>
        /// <param name="term">Takes in the term that we want to update</param>
        public void UpdateTermFrequencyForDoc(string term)
        {

            if (termFrequency.ContainsKey(term))
            {
                termFrequency[term] += 1;
            }
            else
            {
                termFrequency.Add(term, 1);
            }

        }

        /// <summary>
        /// Applies the mathematical rule that we are using to calculate the document weight
        /// </summary>
        public void CalculateDocWeight()
        {
            double temp = 0;
            foreach (int value in termFrequency.Values)
            {
                temp = temp + Math.Pow((1 + Math.Log(value)), 2);
            }
            calculatedDocWeights.Add(Math.Sqrt(temp));
            //clear frequency map for next iteration of document
            termFrequency.Clear();
        }


        /// <summary>
        /// Gets all the document weights saved in memory
        /// </summary>
        /// <returns></returns>
        public IList<double> GetAllDocWeights()
        {
            return calculatedDocWeights;
        }


        /// <summary>
        /// Write dictionaries to disk
        /// </summary>
        public void Save()
        {
            onDiskPostingMap.Save(hashMap, Indexer.path, "Postings");
            onDiskTermFrequencyMap.Save(termFrequency, Indexer.path, "TermFrequency");
            this.WriteDocWeights();
            hashMap.Clear();
            termFrequency.Clear();
        }

        ///<sumary>
        /// Writes 8-byte values of document weights to docWeights.bin 
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to a directory where 'docWeights.bin' be saved</param>
        /// <returns>the list of starting byte positions of each doc weight in docWeights.bin</returns>
        public List<long> WriteDocWeights()
        {
            string filePath = Indexer.path + "docWeights.bin";
            File.Create(filePath).Dispose();
            List<long> startBytes = new List<long>();

            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append)))
            {
                foreach (double weight in this.GetAllDocWeights())
                {
                    startBytes.Add(writer.BaseStream.Length);
                    writer.Write(BitConverter.DoubleToInt64Bits(weight));
                }

                Console.WriteLine($"docWeights.bin  {writer.BaseStream.Length} bytes");
            }

            return startBytes;
        }

        /// <summary>
        /// Uses the document id to access the docWeights.bin file to retrieve the corresponding L_{d} value
        /// </summary>
        /// <param name="docId"></param>
        /// <returns></returns>
        private double GetDocumentWeight(int docId)
        {
            string filePath = Indexer.path + "docWeights.bin";


            using (BinaryReader docWeightsReader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                int startByte = docId * 8;

                //Jump to the starting byte
                docWeightsReader.BaseStream.Seek(startByte, SeekOrigin.Begin);
                //Read a document weight and convert it
                double docWeight = BitConverter.Int64BitsToDouble(docWeightsReader.ReadInt64());

                return docWeight;
            }
        }

        /// <summary>
        /// Method that takes in the query and returns a list of the top ten ranking documents
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList<MaxPriorityQueue.InvertedIndex> GetRankedDocuments(List<string> query)
        {

            //Build the Accumulator Hashmap
            BuildAccumulator(query);

            //Build Priority Queue using the Accumulator divided by L_{d}  
            MaxPriorityQueue pq = BuildPriorityQueue();
            Accumulator.Clear();
            //Retrieve Top Ten Documents and Return to Back End
            return pq.RetrieveTopTen();

        }

        /// <summary>
        /// Builds the Accumulator hashmap for the query to retrieve top 10 documents
        /// </summary>
        /// <param name="query"></param>
        private void BuildAccumulator(List<string> query)
        {
            //w_{q,t}
            double query2TermWeight;
            //w_{d,t}
            double doc2TermWeight;
            //stores temporary Accumulator value that will be added to the accumulator hashmap
            double docAccumulator;
            //gets path to access on disk file
            string path = Indexer.path;

            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {
                //posting list of a term grabbed from the On Disk file
                List<Posting> postings = onDiskPostingMap.Get(term, path, "Postings");

                if (postings != default(List<Posting>))
                {
                    int docFrequency = postings.Count;

                    //implements formula for w_{q,t}
                    query2TermWeight = (double)Math.Log(1 + (double)(this.GetCorpusSize(path) / docFrequency));

                    foreach (Posting post in postings)
                    {
                        //implements formula for w_{d,t}
                        doc2TermWeight = (double)(1 + (double)Math.Log(post.Positions.Count)); //TermFrequency = post.Positions.Count

                        //the A_{d} value for a specific term in that document
                        docAccumulator = query2TermWeight * doc2TermWeight;

                        //if the A_{d} value exists on the hashmap increase its value else create a new key-value pair
                        if (Accumulator.ContainsKey(post.DocumentId))
                        {
                            Accumulator[post.DocumentId] += docAccumulator;
                        }
                        else
                        {
                            Accumulator.Add(post.DocumentId, docAccumulator);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates a new priority queue by inserting the rank of the document and document id 
        /// 
        /// </summary>
        /// <returns> a priority queue with max heap property</returns>
        private MaxPriorityQueue BuildPriorityQueue()
        {
            //temporary variable to hold the doc weight
            double tempDocWeight;
            //temporary variable to hold the final ranking value of that document
            double finalRank;

            //Make a new priority queue
            MaxPriorityQueue priorityQueue = new MaxPriorityQueue();

            //for every key value in the Accumulator divide A_{d} by L_{d}
            foreach (KeyValuePair<int, double> candidate in Accumulator)
            {
                //get document weight by id from docWeights.bin file
                tempDocWeight = GetDocumentWeight(candidate.Key);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = (double)candidate.Value / tempDocWeight;

                //add to list to perform priority queue on 
                priorityQueue.MaxHeapInsert(finalRank, candidate.Key);
            }

            return priorityQueue;

        }

        /// <summary>
        /// Count the number of documents from index
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private int GetCorpusSize(string path)
        {
            int max = 0;
            List<List<Posting>> result = onDiskPostingMap.GetValues(path, "Postings").ToList();
            foreach (List<Posting> postingList in result)
            {
                foreach (Posting p in postingList)
                {
                    max = (p.DocumentId > max) ? p.DocumentId + 1 : max;
                }
            }
            return max;

        }
    }
}
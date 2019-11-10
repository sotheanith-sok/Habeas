/// <param name="term">a processed string</param>
using System.Collections.Generic;
using System.Linq;
using Search.Query;
using System;
using Search.OnDiskDataStructure;
using System.IO;
namespace Search.Index
{
    public class SpecialIndex : IIndex
    {
        //Hashmap is used to store the index. O(1)
        //Dictionary in C# is equivalent to HashMap in Java.
        private readonly SortedDictionary<string, List<Posting>> hashMap;

        //HashMap used to store termFrequency of current Document
        private readonly SortedDictionary<string, int> termFrequency;


        private static List<double> calculatedDocWeights;


        private OnDiskDictionary<string, List<Posting>> onDiskPostingMap;
        private OnDiskDictionary<string, int> onDiskTermFrequencyMap;

        private OnDiskDictionary<int, int> onDiskDocWeight;


        private Dictionary<int, double> Accumulator; //stores the document id with its corresponding rank  [docId -> A_{docID}]


        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public SpecialIndex(string path)
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
            //TODO: change to retrieve postings without regard to position for ranked retrieval?
            List<IList<Posting>> postingLists = new List<IList<Posting>>();
            foreach (string term in terms)
            {
                List<Posting> result = onDiskPostingMap.Get(term, Indexer.path, "Postings");
                if (default(List<Posting>) == result)
                {
                    postingLists.Add(new List<Posting>());
                }
                else
                {
                    postingLists.Add(result);
                }
            }
            return Merge.OrMerge(postingLists);
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

        private double GetDocumentWeight(int docId)
        {
            string filePath = Indexer.path + "docWeights.bin";
        

            using (BinaryReader docWeightsReader = new BinaryReader(File.Open(filePath,FileMode.Open)))
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

        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public IList<MaxPriorityQueue.InvertedIndex> GetRankedDocuments(string[] query)
        {

            //Build the Accumulator Hashmap
            BuildAccumulator(query);

            //Build Priority Queue using the Accumulator divided by L_{d}  
            MaxPriorityQueue pq = BuildPriorityQueue();

            //Retrieve Top Ten Documents and Return to Back End
            return pq.RetrieveTopTen();

        }

        private void BuildAccumulator(string[] query)
        {
            double query2TermWeight;
            double doc2TermWeight;
            double docAccumulator;
            string path = Indexer.path;

            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {

                List<Posting> postings = onDiskPostingMap.Get(term, path, "Postings");
                int docFrequency = postings.Count;

                query2TermWeight = Math.Log(1 + Indexer.corpusSize / docFrequency);


                foreach (Posting post in postings)
                {
                    doc2TermWeight = 1 + Math.Log(post.Positions.Count); //TermFrequency = post.Positions.Count
                    docAccumulator = query2TermWeight * doc2TermWeight;

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
        private MaxPriorityQueue BuildPriorityQueue()
        {

            double tempDocWeight;
            double finalRank;

            MaxPriorityQueue priorityQueue = new MaxPriorityQueue();
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
    }
}
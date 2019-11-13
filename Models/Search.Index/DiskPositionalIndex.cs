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

        //hashmap for docweights posting type list

        private readonly SortedDictionary<int, PostingDocWeight> docWeigthsHashMap;




        //HashMap used to store termFrequency of current Document
        private readonly SortedDictionary<string, int> termFrequency;

        //maintains a list of the docWeights to store in the docWeights.bin file
        private readonly SortedDictionary<int, double> calculatedDocWeights;

        //maintains a hashmap to store average term frequency of current document
        private readonly SortedDictionary<int, double> averageTermFreqPerDoc;

        private readonly SortedDictionary<int, int> tokensPerDocument;

        private readonly SortedDictionary<int, int> docByteSize;





        //maintains the hashmap for the posting list for a specific term
        private OnDiskDictionary<string, List<Posting>> onDiskPostingMap;


        //maintains a hashmap for the document weight for a specific document id
        private OnDiskDictionary<int, PostingDocWeight> onDiskDocWeight;


        private double averageDocLength;




        public class PostingDocWeight
        {

            private double docWeights { get; set; }
            private int docLength { get; set; }
            private int docByteSize { get; set; }
            private double averageTermFreq { get; set; }

            public PostingDocWeight(double docWeight, int docLength, int docByteSize, double averageTermFreq)
            {
                this.docWeights = docWeight;
                this.docLength = docLength;
                this.docByteSize = docByteSize;
                this.averageTermFreq = averageTermFreq;
            }

            public double GetDocWeight()
            {
                return this.docWeights;
            }

            public int GetDocTokenCount()
            {
                return this.docLength;
            }

            public int GetDocByteSize()
            {
                return this.docByteSize;
            }
            public double GetDocAveTermFreq()
            {
                return this.averageTermFreq;
            }

        }

        /// <summary>
        /// Constructs a hash table.
        /// </summary>
        public DiskPositionalIndex(string path)
        {

            termFrequency = new SortedDictionary<string, int>();
            tokensPerDocument = new SortedDictionary<int, int>();
            docByteSize = new SortedDictionary<int, int>();
            calculatedDocWeights = new SortedDictionary<int, double>();
            averageTermFreqPerDoc = new SortedDictionary<int, double>();

            hashMap = new SortedDictionary<string, List<Posting>>();
            docWeigthsHashMap = new SortedDictionary<int, PostingDocWeight>();

            onDiskPostingMap = new OnDiskDictionary<string, List<Posting>>(path, "InvertedIndex", new StringEncoderDecoder(), new PostingListEncoderDecoder());
            onDiskDocWeight = new OnDiskDictionary<int, PostingDocWeight>(path, "docWeights", new IntEncoderDecoder(), new PostingDocWeightEncoderDecoder());

        }

        public List<PostingDocWeight> GetPostingDocWeights(List<string> query)
        {
            //how to find only the docids of a given query
            IList<Posting> postings = GetPostings(query);

            List<int> documentIds = new List<int>();
            foreach(Posting p in postings)
            {
                documentIds.Add(p.DocumentId);
            }

            List<PostingDocWeight> finalList = new List<PostingDocWeight>();
            finalList = onDiskDocWeight.Get(documentIds);
            return finalList;
        }


        /// <summary>
        /// Gets Postings of a given term from in-memory index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {

            List<Posting> result = onDiskPostingMap.Get(term);
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
            List<List<Posting>> postingLists = onDiskPostingMap.Get(terms);
            postingLists.RemoveAll(item => item == default(List<Posting>));
            if (postingLists.Count == 0)
            {
                return new List<Posting>();
            }
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
            List<string> vocabulary = onDiskPostingMap.GetKeys().ToList();
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
        public void CalculateDocWeight(int docID)
        {
            double temp = 0;
            foreach (int value in termFrequency.Values)
            {
                temp = temp + Math.Pow((1 + Math.Log(value)), 2);
            }

            //adds to list of doc weights to save later onto disk
            calculatedDocWeights.Add(docID, Math.Sqrt(temp));

            //clear frequency map for next iteration of document
            termFrequency.Clear();
        }


        /// <summary>
        /// calcuates the average token frequency of a particulat document
        /// </summary>
        public double calculateAverageDocLength()
        {
            double average = 0;
            foreach (KeyValuePair<int, int> docTokens in tokensPerDocument)
            {
                average = average + docTokens.Value;
            }
            average = (double)average / tokensPerDocument.Count;

            this.averageDocLength = average;
            return average;
        }



        /// <summary>
        /// Write dictionaries to disk
        /// </summary>
        public void Save()
        {
            onDiskPostingMap.Save(hashMap);
            this.WriteDocWeights();
            onDiskDocWeight.Save(docWeigthsHashMap);


            hashMap.Clear();
            docWeigthsHashMap.Clear();
            termFrequency.Clear();
            calculatedDocWeights.Clear();
            docByteSize.Clear();
            tokensPerDocument.Clear();
            averageTermFreqPerDoc.Clear();
        }

        ///<sumary>
        /// Writes 8-byte values of document weights to docWeights.bin 
        /// </summary>
        /// <param name="index">the index to write</param>
        /// <param name="dirPath">the absolute path to a directory where 'docWeights.bin' be saved</param>
        /// <returns>the list of starting byte positions of each doc weight in docWeights.bin</returns>
        public void WriteDocWeights()
        {
            double tempDocWeight;
            int tempDocLength;
            double tempAverTermFreq;
            foreach (KeyValuePair<int, int> doc in docByteSize)
            {

                tempDocWeight = calculatedDocWeights[doc.Key];
                tempDocLength = tokensPerDocument[doc.Key];
                tempAverTermFreq = averageTermFreqPerDoc[doc.Key];

                PostingDocWeight tempPostDocWeight = new PostingDocWeight(tempDocWeight, tempDocLength, doc.Value, tempAverTermFreq);

                docWeigthsHashMap.Add(doc.Key, tempPostDocWeight);

            }

        }

        public void CalcAveTermFreq(int docID)
        {
            int sum = 0;
            foreach (int termFreq in this.termFrequency.Values)
            {
                sum = sum + termFreq;
            }

            double averageTermFreq = (double)sum / this.termFrequency.Count;

            averageTermFreqPerDoc.Add(docID, averageTermFreq);

        }

        public void AddTokensPerDocument(int docId, int tokenCount)
        {

            tokensPerDocument.Add(docId, tokenCount);


        }

        public void AddByteSize(int docID, int fileSizeInBytes)
        {
            docByteSize.Add(docID, fileSizeInBytes);
        }




        /// <summary>
        /// Gets Postings of a given list of terms from in-memory index.
        /// This and-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a and-merged posting list</return>
        public IList<Posting> GetPostingsUsingAndMerge(List<string> terms)
        {
            List<List<Posting>> postingLists = onDiskPostingMap.Get(terms);
            postingLists.RemoveAll(item => item == default(List<Posting>));
            if (postingLists.Count == 0)
            {
                return new List<Posting>();
            }
            return Merge.AndMerge(new List<IList<Posting>>(postingLists));
        }
    }

}








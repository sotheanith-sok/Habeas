using System.Collections.Generic;
using System;
using System.IO;
using Search.Query;

namespace Search.Index
{
    /// <summary>
    /// Reads the On-disk positional inverted index that was constructed by DiskIndexWriter
    /// </summary>
    public class DiskPositionalIndex : IIndex, IDisposable
    {
        private string dirPath;
        private BinaryReader vocabReader;
        private BinaryReader postingReader;
        private long[] vocabTable;  // [t1Start, p1Start, t2Start, p2Start, ...]
        private BinaryReader docWeightsReader;

        private Dictionary<int, double> Accumulator; //stores the document id with its corresponding rank  [docId -> A_{docID}]

        private List<int> HighestRankDocs; // stores the top 10 ranking documents using heap list

        /// <summary>
        /// Opens an on-disk positional inverted index that was constructed in the given path
        /// </summary>
        /// <param name="dirPath">the absolute path to the folder where binary files are saved</param>
        public DiskPositionalIndex(string dirPath)
        {
            try
            {
                this.dirPath = dirPath;
                vocabReader = new BinaryReader(File.OpenRead(dirPath + "vocab.bin"));
                postingReader = new BinaryReader(File.OpenRead(dirPath + "postings.bin"));
                vocabTable = ReadVocabTable(dirPath);
                docWeightsReader = new BinaryReader(File.OpenRead(dirPath + "docWeights.bin"));
                Console.WriteLine("Opened 3 binary files.");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Gets Postings only with docIDs from a given term from on-disk index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {
            long postingStart = BinarySearchVocabulary(term);
            return ReadPostings(postingStart, false);
        }

        /// <summary>
        /// Gets Postings only with docIDs from a given list of terms from on-disk index.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {
            var postingLists = new List<IList<Posting>>();
            foreach (string term in terms)
            {
                postingLists.Add(GetPostings(term));
            }
            return Merge.OrMerge(postingLists);
        }

        /// <summary>
        /// Gets Postings with positions from a given term from on-disk index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPositionalPostings(string term)
        {
            long postingStart = BinarySearchVocabulary(term);
            return ReadPostings(postingStart, true);
        }

        /// <summary>
        /// Gets Postings with positions from a given list of terms from on-disk index.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPositionalPostings(List<string> terms)
        {
            var postingLists = new List<IList<Posting>>();
            foreach (string term in terms)
            {
                postingLists.Add(GetPositionalPostings(term));
            }
            return Merge.OrMerge(postingLists);
        }

        /// <summary>
        /// Gets a sorted list of all vocabularies from index.
        /// </summary>
        public IReadOnlyList<string> GetVocabulary()
        {
            List<string> finalList = new List<string>();
            int termCount = GetTermCount();
            for (int i = 0; i < termCount; i++)
            {
                finalList.Add(vocabReader.ReadString());
            }
            return finalList;
        }

        /// <summary>
        /// Locates the byte position of the postings for the given term.
        /// For example, binarySearchVocabulary("angel") will return the byte position
        /// to seek to in postings.bin to find the postings for "angel".
        /// </summary>
        /// <param name="term">the term to find</param>
        /// <returns>the byte position of the postings</returns>
        private long BinarySearchVocabulary(string term)
        {
            // Do a binary search over the vocabulary,
            // using the vocabTable and the vocabReader(vocab.bin).
            int i = 0;
            int j = vocabTable.Length / 2 - 1;
            while (i <= j)
            {
                try
                {
                    int m = (i + j) / 2;
                    long termStartByte = vocabTable[m * 2];
                    vocabReader.BaseStream.Seek(termStartByte, SeekOrigin.Begin);
                    string termFromFile = vocabReader.ReadString();

                    int compareValue = term.CompareTo(termFromFile);
                    if (compareValue == 0)
                    {
                        // found it!
                        return vocabTable[m * 2 + 1];
                    }
                    else if (compareValue < 0)
                    {
                        j = m - 1;
                    }
                    else
                    {
                        i = m + 1;
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            return -1;
        }

        /// <summary>
        /// Reads the file vocabTable.bin into memory
        /// </summary>
        /// <param name="dirPath">the absolute path to the folder where binary index files are saved</param>
        /// <returns>the long array of vocabTable</returns>
        private static long[] ReadVocabTable(string dirPath)
        {
            try
            {
                var vocabTable = new List<long>();
                var reader = new BinaryReader(File.OpenRead(dirPath + "vocabTable.bin"));
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    vocabTable.Add(reader.ReadInt64()); //termStart
                    vocabTable.Add(reader.ReadInt64()); //postingStart
                }
                reader.Close();
                return vocabTable.ToArray();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return null;
        }


        /// <summary>
        /// Gets the term count of the vocabulary(vocab.bin)
        /// </summary>
        /// <returns>the size of the vocabulary</returns>
        public int GetTermCount()
        {
            return vocabTable.Length / 2;
        }


        /// <summary>
        /// Read postings without positions for a term from postings.bin
        /// </summary>
        /// <param name="startByte">the starting byte of a posting list within postings.bin</param>
        /// <param name="wantPositions">Do you want positions? or not?</param>
        /// <returns>a posting list</returns>
        private IList<Posting> ReadPostings(long startByte, bool wantPositions)
        {
            // Read and construct a posting list from postings.bin
            // < df, (docID tf p1 p2 p3), (doc2 tf p1 p2), ... >
            // docIDs and positions are written as gap)

            //0. Jump to the starting byte
            postingReader.BaseStream.Seek(startByte, SeekOrigin.Begin);

            IList<Posting> postings = new List<Posting>();

            //1. Read document frequency
            int docFrequency = postingReader.ReadInt32();

            int prevDocID = 0;
            for (int i = 0; i < docFrequency; i++)         //for each posting
            {
                //2. Read documentID using gap
                int docID = prevDocID + postingReader.ReadInt32();

                List<int> positions = new List<int>();

                //3. Read term frequency
                int termFrequency = postingReader.ReadInt32();

                if (wantPositions)
                {
                    //4. Read positions using gap
                    int prevPos = 0;
                    for (int j = 0; j < termFrequency; j++)    //for each position
                    {
                        int pos = prevPos + postingReader.ReadInt32();
                        positions.Add(pos);
                        prevPos = pos;  //update prevPos
                    }
                }
                else
                {
                    //Skip the positions
                    postingReader.BaseStream.Seek(termFrequency * sizeof(int), SeekOrigin.Current);
                }

                //Insert a posting to the posting list
                postings.Add(new Posting(docID, positions));

                prevDocID = docID;  //update prevDocID
            }

            return postings;
        }


        /// <summary>
        /// Gets the document weight from docWeights.bin
        /// </summary>
        /// <param name="docId">the docId of the document to get weight of</param>
        /// <returns>the document weight</returns>
        private double GetDocumentWeight(int docId)
        {
            int startByte = docId * 8;

            //Jump to the starting byte
            docWeightsReader.BaseStream.Seek(startByte, SeekOrigin.Begin);
            //Read a document weight and convert it
            double docWeight = BitConverter.Int64BitsToDouble(docWeightsReader.ReadInt64());

            return docWeight;
        }


        /// <summary>
        /// Retrives all the the document weights from the DocWeights.bin file.
        /// </summary>
        /// <returns>A list of the document weights </returns>
        public IList<double> GetAllDocWeights()
        {

            IList<double> allDocWeights = new List<double>();
            docWeightsReader.BaseStream.Seek(0, SeekOrigin.Begin);

            while (docWeightsReader.BaseStream.Position != docWeightsReader.BaseStream.Length)
            {
                double docWeight = BitConverter.Int64BitsToDouble(docWeightsReader.ReadInt64());
                allDocWeights.Add(docWeight);

            }

            return allDocWeights;

        }
        /// <summary>
        /// Dispose all binary readers
        /// </summary>



        public void RankedDocuments(List<string> query, int corpusSize)
        {

            double query2TermWeight;
            double doc2TermWeight;
            double docAccumulator;

            //caculate accumulated Value for each relevant document A_{d}
            foreach (string term in query)
            {
                long startByte = BinarySearchVocabulary(term);

                //0. Jump to the starting byte
                postingReader.BaseStream.Seek(startByte, SeekOrigin.Begin);



                //1. Read document frequency
                int docFrequency = postingReader.ReadInt32();

                query2TermWeight = Math.Log(1 + corpusSize / docFrequency);

                int prevDocID = 0;
                for (int i = 0; i < docFrequency; i++)         //for each posting
                {
                    //2. Read documentID using gap
                    int docID = prevDocID + postingReader.ReadInt32();

                    //3. Read term frequency
                    int termFrequency = postingReader.ReadInt32();

                    doc2TermWeight = 1 + Math.Log(termFrequency);
                    docAccumulator = query2TermWeight * doc2TermWeight;

                    if (Accumulator.ContainsKey(docID))
                    {
                        Accumulator[docID] += docAccumulator;
                    }
                    else
                    {
                        Accumulator.Add(docID, docAccumulator);
                    }

                    //Skip the positions
                    postingReader.BaseStream.Seek(termFrequency * sizeof(int), SeekOrigin.Current);

                    prevDocID = docID;  //update prevDocID
                }
            }

            //For every document in the Accumulator divid by L_{d}

            double tempDocWeight;
            double finalRank;
            foreach (KeyValuePair<int, double> candidate in Accumulator)
            {
                //get document weight by id from docWeights.bin file
                tempDocWeight = GetDocumentWeight(candidate.Key);

                // divide Accumulated Value A_{d} by L_{d} 
                finalRank = candidate.Value / tempDocWeight;

                //TO-DO implement binary heap priority queue
            }
        }

        public void Dispose()
        {
            vocabReader?.Dispose();
            postingReader?.Dispose();
            docWeightsReader?.Dispose();

            Console.WriteLine("Disposed all binary files.");
        }
    }

}

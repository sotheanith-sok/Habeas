using System.Collections.Generic;
using System;
using System.IO;
using Search.Query;

namespace Search.Index
{
    /// <summary>
    /// 
    /// </summary>
    public class DiskPositionalIndex : IIndex, IDisposable
    {

        private BinaryReader vocabReader;
        private BinaryReader postingReader;
        private BinaryReader vocabTableReader;
        private BinaryReader docWeightsReader;

        /// <summary>
        /// Constructs DiskPositionalIndex and opens the BinaryReaders
        /// </summary>
        /// <param name="dirPath">the absolute path to the directory where binary files are saved</param>
        public DiskPositionalIndex(string dirPath)
        {

            string vocabPath = dirPath + "vocab.bin";
            string postingPath = dirPath + "postings.bin";
            string vocabTablePath = dirPath + "vocabTable.bin";
            string docWeightPath = dirPath + "docWeights.bin";

            //TODO: handle exceptions
            vocabReader = new BinaryReader(File.Open(vocabPath, FileMode.Open));
            postingReader = new BinaryReader(File.Open(postingPath, FileMode.Open));
            vocabTableReader = new BinaryReader(File.Open(vocabTablePath, FileMode.Open));
            docWeightsReader = new BinaryReader(File.Open(docWeightPath, FileMode.Open));

        }

        /// <summary>
        /// Gets Postings only with docIDs from a given term from on-disk index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {
            //TODO: implement this

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets Postings only with docIDs from a given list of terms from on-disk index.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {
            List<IList<Posting>> postingLists = new List<IList<Posting>>();
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
            //TODO: implement this

            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets Postings with positions from a given list of terms from on-disk index.
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPositionalPostings(List<string> terms)
        {
            List<IList<Posting>> postingLists = new List<IList<Posting>>();
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
        /// Gets the term count from vocab.bin
        /// </summary>
        /// <returns>the size of the vocabulary</returns>
        public int GetTermCount()
        {
            //TODO: test
            return (int)(vocabTableReader.BaseStream.Length) / 2;
        }

        /// <summary>
        /// Gets the document weight from docWeights.bin
        /// </summary>
        /// <param name="docId">the docId of the document to get weight of</param>
        /// <returns>the document weight</returns>
        public double GetDocumentWeight(int docId)
        {
            int startByte = docId * 8;
            
            //Jump to the starting byte
            docWeightsReader.BaseStream.Seek(startByte, SeekOrigin.Begin);
            //Read a document weight and convert it
            double docWeight = BitConverter.Int64BitsToDouble(docWeightsReader.ReadInt64());
            
            return docWeight;
        }


        /// <summary>
        /// Read postings without positions for a term from postings.bin
        /// </summary>
        /// <param name="startByte">the starting byte of a posting list within postings.bin</param>
        /// <param name="wantPositions">Do you want positions? or not?</param>
        /// <returns>a posting list</returns>
        public IList<Posting> ReadPostings(long startByte, bool wantPositions)
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

                if(wantPositions)
                {
                    //4. Read positions using gap
                    int prevPos = 0;
                    for(int j=0; j < termFrequency; j++)    //for each position
                    {
                        int pos = prevPos + postingReader.ReadInt32();
                        positions.Add(pos);
                        prevPos = pos;  //update prevPos
                    }
                }
                else {
                    //Skip the positions
                    postingReader.BaseStream.Seek(termFrequency*sizeof(int), SeekOrigin.Current);
                }
                
                //Insert a posting to the posting list
                postings.Add(new Posting(docID, positions));

                prevDocID = docID;  //update prevDocID
            }

            return postings;
        }

        /// <summary>
        /// Dispose all binary readers
        /// </summary>
        public void Dispose()
        {
            vocabReader?.Dispose();
            postingReader?.Dispose();
            vocabTableReader?.Dispose();
            docWeightsReader?.Dispose();
            
            Console.WriteLine("Disposed all binary");
        }
    }

}

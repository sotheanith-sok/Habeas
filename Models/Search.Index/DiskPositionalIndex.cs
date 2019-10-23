using System.Collections.Generic;
using System;
using System.IO;
using UnitTests;

namespace Search.Index
{
    /// <summary>
    /// 
    /// </summary>
    public class DiskPositionalIndex : IIndex, IDisposable
    {

        BinaryReader vocabReader;
        BinaryReader postingReader;
        BinaryReader vocabTableReader;

        /// <summary>
        /// 
        /// </summary>
        public DiskPositionalIndex(string FolderPath)
        {

            String VocabPath = FolderPath + "vocab.bin";
            String PostingPath = FolderPath + "postings.bin";
            String VocabTablePath = FolderPath + "vocabTable.bin";

            vocabReader = new BinaryReader(File.Open(VocabPath, FileMode.Open));
            postingReader = new BinaryReader(File.Open(PostingPath, FileMode.Open));
            vocabTableReader = new BinaryReader(File.Open(VocabTablePath, FileMode.Open));

        }

        /// <summary>
        /// Gets Postings only with docIDs from a given term from on-disk index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostings(string term)
        {

            return new List<Posting>();

        }

        /// <summary>
        /// Gets Postings only with docIDs from a given list of terms from on-disk index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostings(List<string> terms)
        {

            return new List<Posting>();
        }

        /// <summary>
        /// Gets Postings of a given term from index.
        /// </summary>
        /// <param name="term">a processed string</param>
        /// <return>a posting list</return>
        public IList<Posting> GetPostingsPositional(string term)
        {


            return new List<Posting>();

        }

        /// <summary>
        /// Gets Postings of a given list of terms from index.
        /// This or-merge the all the results from the multiple terms
        /// </summary>
        /// <param name="terms">a list of processed strings</param>
        /// <return>a or-merged posting list</return>
        public IList<Posting> GetPostingsPositional(List<string> terms)
        {

            return new List<Posting>();
        }

        /// <summary>
        /// Gets a sorted list of all vocabularies from index.
        /// </summary>
        public IReadOnlyList<string> GetVocabulary()
        {
            List<string> finalList = new List<String>();
            int termCount = getTermCount();
            for(int i=0; i < termCount; i++){
                finalList.Add(vocabReader.ReadString());
            }
            return finalList;
        }

        public int getTermCount()
        {
            //TODO: test
            return (int)(vocabTableReader.BaseStream.Length)/2;
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
            for(int i=0; i < docFrequency; i++)         //for each posting
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

            UnitTest.PrintPostingResult(postings);

            return postings;
        }


        public void Dispose()
        {
            vocabReader?.Dispose();
            postingReader?.Dispose();
            vocabTableReader?.Dispose();
        }
    }

}

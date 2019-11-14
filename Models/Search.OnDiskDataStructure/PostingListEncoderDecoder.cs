using System.Collections.Generic;
using System;
using Search.Index;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of IEncoderDecoder for List<Posting>
    /// </summary>
    public class PostingListEncoderDecoder : IEncoderDecoder<List<Posting>>
    {

        /// <summary>
        /// Encode list of integers to bytes
        /// </summary>
        /// <param name="postings">a posting list</param>
        /// <returns>encoded bytes stream</returns>
        public byte[] Encoding(List<Posting> postings)
        {
            List<int> concat = new List<int>();

            //1. Write document frequency
            concat.Add(postings.Count);

            int previousDocID = 0;
            foreach (Posting p in postings)
            {
                //2. Write docID using gap
                concat.Add(p.DocumentId - previousDocID); //4byte integer per docID

                List<int> positions = p.Positions;

                //3. Write term frequency (# of positions)
                concat.Add(positions.Count);              //4byte integer per term frequency

                //4. Write positions using gap
                int previousPos = 0;
                foreach (int pos in positions)
                {
                    concat.Add(pos - previousPos);        //4byte integer per position
                    previousPos = pos;
                }

                previousDocID = p.DocumentId;
            }

            return VariableBytes.Compress(concat);
        }

        /// <summary>
        /// Converts an byte array to a list of postings for a term.
        /// The byte array should follow the form 
        /// < df, (docID tf p1 p2 p3), (doc2 tf p1 p2), ... >
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<Posting> Decoding(byte[] value)
        {
            List<int> integers = VariableBytes.DecompressToInts(value);
            Console.WriteLine(string.Join(" ", integers));
            // Read and construct a posting list from bytes from postings.bin
            // < df, (docID tf p1 p2 p3), (doc2 tf p1 p2), ... >
            // docIDs and positions are written as gap)

            List<Posting> postings = new List<Posting>();
            int index = 0;
            //1. Read document frequency
            int docFrequency = integers[index++];

            int prevDocID = 0;
            for (int i = 0; i < docFrequency; i++)         //for each posting
            {
                //2. Read documentID using gap
                int docID = prevDocID + integers[index++];

                List<int> positions = new List<int>();

                //3. Read term frequency
                int termFrequency = integers[index++];

                //4. Read positions using gap
                int prevPos = 0;
                for (int j = 0; j < termFrequency; j++)    //for each position
                {
                    int pos = prevPos + integers[index++];
                    positions.Add(pos);
                    prevPos = pos;  //update prevPos
                }

                //Insert a posting to the posting list
                postings.Add(new Posting(docID, positions));

                prevDocID = docID;  //update prevDocID
            }

            return postings;
        }

    }
}
using System.Collections.Generic;
using System;
using Search.Index;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of IEncoderDecoder for MaxPriorityQueue.InvertedIndex
    /// </summary>
    public class InvertedIndexEncoderDecoder : IEncoderDecoder<List<MaxPriorityQueue.InvertedIndex>>
    {

        /// <summary>
        /// Encode list of integers to bytes
        /// </summary>
        /// <param name="value">List of integers</param>
        /// <returns>bytes array</returns>
        public byte[] Encoding(List<MaxPriorityQueue.InvertedIndex> queue)
        {
            queue.Sort(
            delegate (MaxPriorityQueue.InvertedIndex Item1, MaxPriorityQueue.InvertedIndex Item2)
            {
                int docID1 = Item1.GetDocumentId();
                int docID2 = Item2.GetDocumentId();
                if (docID1 < docID2)
                {
                    return -1;
                }
                if (docID2 < docID1)
                {
                    return 1;
                }
                else
                    return 0;

            });
            List<int> concat = new List<int>();

            //1. Write the document frequency
            concat.Add(queue.Count);

            int previousDocId = 0;

            int documentID;
            int termFreq;

            foreach (MaxPriorityQueue.InvertedIndex item in queue)
            {

                documentID = item.GetDocumentId();

                //1. Write the document id using gaps
                concat.Add(documentID - previousDocId);

                termFreq = item.GetTermFreq();

                //3.Write the term frequency 
                concat.Add(termFreq);

                previousDocId = documentID;
            }

            return VariableBytes.Compress(concat);
        }

        /// <summary>
        /// Decode bytes to Inverted
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<MaxPriorityQueue.InvertedIndex> Decoding(byte[] value)
        {
            List<int> integers = VariableBytes.DecompressToInts(value);
            
            
            List<MaxPriorityQueue.InvertedIndex> tierPostings = new List<MaxPriorityQueue.InvertedIndex>();

            int index = 0;

            int docFrequency = integers[index++];

            int previousDocId = 0;

            for (int i = 0; i < docFrequency; i++ )
            {
                //Read documentID using gap
                int docID = previousDocId + integers[index++];
                int termFreq = integers[index++];
                tierPostings.Add(new MaxPriorityQueue.InvertedIndex(termFreq, docID));
                previousDocId = docID;
            }

            return tierPostings;
        }
    }
}
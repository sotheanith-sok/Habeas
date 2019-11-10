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
        /// <param name="value">List of integers</param>
        /// <returns>bytes array</returns>
        public byte[] Encoding(List<Posting> value)
        {
            List<byte> byteValue = new List<byte>();

            //1. Write document frequency
            byteValue.AddRange(BitConverter.GetBytes(value.Count));

            int previousDocID = 0;
            foreach (Posting p in value)
            {
                //2. Write docID using gap
                byteValue.AddRange(BitConverter.GetBytes(p.DocumentId - previousDocID)); //4byte integer per docID

                List<int> positions = p.Positions;

                //3. Write term frequency (# of positions)
                byteValue.AddRange(BitConverter.GetBytes(positions.Count));              //4byte integer per term frequency

                //4. Write positions using gap
                int previousPos = 0;
                foreach (int pos in positions)
                {
                    byteValue.AddRange(BitConverter.GetBytes(pos - previousPos));        //4byte integer per position
                    previousPos = pos;
                }

                previousDocID = p.DocumentId;
            }

            return byteValue.ToArray();
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<Posting> Decoding(byte[] value)
        {
            List<Posting> postingList = new List<Posting>();
            List<Byte> byteList = new List<byte>(value);

            int previousDocID = 0;

            //Skip 4 to disregard document frequence
            int i = 4;

            while (i < value.Length)
            {
                List<int> positions = new List<int>();
                //Get docID 
                int docID = BitConverter.ToInt32(byteList.GetRange(i, 4).ToArray()) + previousDocID;
                i += 4;

                //Get term frequence
                int tf = BitConverter.ToInt32(byteList.GetRange(i, 4).ToArray());
                i += 4;
                int previousPos = 0;

                while (tf > 0)
                {
                    int pos = BitConverter.ToInt32(byteList.GetRange(i, 4).ToArray()) + previousPos;
                    positions.Add(pos);
                    previousPos = pos;
                    tf -= 1;
                    i += 4;
                }
                previousDocID = docID;

                postingList.Add(new Posting(docID, positions));
            }

            return postingList;
        }
    }
}
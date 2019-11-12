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
            List<byte[]> byteList = new List<byte[]>();

            //1. Write document frequency
            byteList.Add(BitConverter.GetBytes(value.Count));

            int previousDocID = 0;
            foreach (Posting p in value)
            {
                //2. Write docID using gap
                byteList.Add(BitConverter.GetBytes(p.DocumentId - previousDocID)); //4byte integer per docID

                List<int> positions = p.Positions;

                //3. Write term frequency (# of positions)
                byteList.Add(BitConverter.GetBytes(positions.Count));              //4byte integer per term frequency


                //4. Write positions using gap
                int previousPos = 0;
                foreach (int pos in positions)
                {
                    byteList.Add(BitConverter.GetBytes(pos - previousPos));        //4byte integer per position
                    previousPos = pos;

                }

                previousDocID = p.DocumentId;
            }

            return Compressor.Compress(byteList);
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<Posting> Decoding(byte[] value)
        {
            List<Posting> postingList = new List<Posting>();
            
            List<byte[]> byteList = Compressor.Decompress(value);

            int previousDocID = 0;

            //Skip 1 to disregard document frequence
            int i = 1;

            while (i < byteList.Count)
            {
                List<int> positions = new List<int>();
                //Get docID 
                byte[] intStorage = new byte[4];
                byteList[i].CopyTo(intStorage, 0);
                int docID = BitConverter.ToInt32(intStorage) + previousDocID;
                i += 1;

                //Get term frequence
                intStorage = new byte[4];
                byteList[i].CopyTo(intStorage, 0);
                int tf = BitConverter.ToInt32(intStorage);
                i += 1;
                int previousPos = 0;

                while (tf > 0)
                {
                    intStorage = new byte[4];
                    byteList[i].CopyTo(intStorage, 0);
                    int pos = BitConverter.ToInt32(intStorage) + previousPos;
                    positions.Add(pos);
                    previousPos = pos;
                    tf -= 1;
                    i += 1;
                }
                previousDocID = docID;

                postingList.Add(new Posting(docID, positions));
            }

            return postingList;
        }
    }
}
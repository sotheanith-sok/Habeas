using System.Collections.Generic;
using System;
using Search.Index;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of IEncoderDecoder for List<Posting>
    /// </summary>
    public class PostingDocWeightEncoderDecoder : IEncoderDecoder<DiskPositionalIndex.PostingDocWeight>
    {

        /// <summary>
        /// Encode list of integers to bytes
        /// </summary>
        /// <param name="value">List of integers</param>
        /// <returns>bytes array</returns>
        public byte[] Encoding(DiskPositionalIndex.PostingDocWeight value)
        {
            List<byte> byteValue = new List<byte>();


            //Write the document weight
            byteValue.AddRange(BitConverter.GetBytes(value.GetDocWeight()));

            byteValue.AddRange(BitConverter.GetBytes(value.GetDocTokenCount()));

            byteValue.AddRange(BitConverter.GetBytes(value.GetDocByteSize()));

            byteValue.AddRange(BitConverter.GetBytes(value.GetDocAveTermFreq()));


            return byteValue.ToArray();
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public DiskPositionalIndex.PostingDocWeight Decoding(byte[] value)
        {

            List<Byte> byteList = new List<byte>(value);

            double docWeight = BitConverter.ToDouble(byteList.GetRange(0, 8).ToArray());
            int docLength = BitConverter.ToInt32(byteList.GetRange(8, 4).ToArray());
            int docByteSize = BitConverter.ToInt32(byteList.GetRange(12, 4).ToArray());
            double docAveTermFreq = BitConverter.ToDouble(byteList.GetRange(16, 8).ToArray());




            DiskPositionalIndex.PostingDocWeight postingDocWeight = new DiskPositionalIndex.PostingDocWeight(docWeight, docLength, docByteSize, docAveTermFreq);
            return postingDocWeight;
        }
    }
}
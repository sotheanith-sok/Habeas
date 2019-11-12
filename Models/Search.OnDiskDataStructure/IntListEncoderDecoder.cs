using System.Collections.Generic;
using System;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of IEncoderDecoder for List<int>
    /// </summary>
    public class IntListEncoderDecoder : IEncoderDecoder<List<int>>
    {

        /// <summary>
        /// Encode list of integers to bytes
        /// </summary>
        /// <param name="value">List of integers</param>
        /// <returns>bytes array</returns>
        public byte[] Encoding(List<int> value)
        {
            return VariableBytes.Encode(value);
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<int> Decoding(byte[] value)
        {
            var byteStream = new VariableBytes.EncodedByteStream(value);
            var decoded = new List<int>();
            while(byteStream.Pos < value.Length)
            {
                decoded.Add(byteStream.ReadDecodedInt());
            }
            return decoded;

        }
    }
}
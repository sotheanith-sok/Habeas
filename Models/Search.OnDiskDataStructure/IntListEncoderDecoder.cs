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
            return VariableBytes.Compress(value);
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<int> Decoding(byte[] value)
        {
            return VariableBytes.DecompressToInts(value);

        }
    }
}
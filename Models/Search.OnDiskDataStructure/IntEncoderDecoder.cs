using System.Collections.Generic;
using System;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of IEncoderDecoder for int
    /// </summary>
    public class IntEncoderDecoder : IEncoderDecoder<int>
    {

        /// <summary>
        /// Encode integers to bytes
        /// </summary>
        /// <param name="value">integers</param>
        /// <returns>bytes array</returns>
        public byte[] Encoding(int value)
        {
            return VariableBytes.Encode(value);
        }

        /// <summary>
        /// Decode bytes to integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>integers</returns>
        public int Decoding(byte[] value)
        {
            return VariableBytes.Decode(value);
        }
    }
}
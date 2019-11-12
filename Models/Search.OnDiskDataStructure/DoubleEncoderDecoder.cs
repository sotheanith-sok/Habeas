using System.Collections.Generic;
using System;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of IEncoderDecoder for int
    /// </summary>
    public class DoubleEncoderDecoder : IEncoderDecoder<double>
    {

        /// <summary>
        /// Encode integers to bytes
        /// </summary>
        /// <param name="value">integers</param>
        /// <returns>bytes array</returns>
        public byte[] Encoding(double value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// Decode bytes to integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>integers</returns>
        public double Decoding(byte[] value)
        {
            return BitConverter.ToDouble(value);
        }
    }
}
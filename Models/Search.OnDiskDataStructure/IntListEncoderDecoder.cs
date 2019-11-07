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
            List<byte> byteValue = new List<byte>();
            foreach (int i in value)
            {
                byteValue.AddRange(BitConverter.GetBytes(i));
            }
            return byteValue.ToArray();
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<int> Decoding(byte[] value)
        {
            List<int> intValue = new List<int>();
            for (int i = 0; i < value.Length; i = i + 4)
            {
                intValue.Add(BitConverter.ToInt32(value, i));
            }
            return intValue;
        }
    }
}
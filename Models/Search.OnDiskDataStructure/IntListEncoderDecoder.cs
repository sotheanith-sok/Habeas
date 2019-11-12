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
            List<byte[]> byteValue = new List<byte[]>();
            foreach (int i in value)
            {
                byteValue.Add(BitConverter.GetBytes(i));
            }
            return Compressor.Compress(byteValue);
        }

        /// <summary>
        /// Decode bytes to list of integers
        /// </summary>
        /// <param name="value">Bytes</param>
        /// <returns>List of integers</returns>
        public List<int> Decoding(byte[] value)
        {
            List<byte[]> byteValue = Compressor.Decompress(value);
            List<int> result = new List<int>();
            while (byteValue.Count > 0)
            {
                byte[] intStorage = new byte[4];
                byteValue[0].CopyTo(intStorage, 0);
                result.Add(BitConverter.ToInt32(intStorage));
                byteValue.RemoveAt(0);
            }
            return result;
        }
    }
}
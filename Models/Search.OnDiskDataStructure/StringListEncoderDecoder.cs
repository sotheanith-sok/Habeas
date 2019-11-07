using System.Collections.Generic;
using System.Linq;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of EncoderDecoder for List<string>
    /// </summary>
    public class StringListEncoderDecoder : IEncoderDecoder<List<string>>
    {
        /// <summary>
        /// Encode list of strings to bytes
        /// </summary>
        /// <param name="value">List of string</param>
        /// <returns>Array of bytes</returns>
        public byte[] Encoding(List<string> value)
        {
            return System.Text.Encoding.UTF8.GetBytes(string.Join(" ", value));
        }

        /// <summary>
        /// Decode bytes to list of strings
        /// </summary>
        /// <param name="value">Array of bytes</param>
        /// <returns>List of strings</returns>
        public List<string> Decoding(byte[] value)
        {
            return System.Text.Encoding.UTF8.GetString(value).Split(" ").ToList(); ;
        }
    }
}
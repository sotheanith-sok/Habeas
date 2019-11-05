namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implementation of EncoderDecoder for string
    /// </summary>
    public class StringEncoderDecoder : IEncoderDecoder<string>
    {

        /// <summary>
        /// Encode string to bytes
        /// </summary>
        /// <param name="value">string value</param>
        /// <returns>Array of byte</returns>
        public byte[] Encoding(string value)
        {
            return System.Text.Encoding.UTF8.GetBytes(value);
        }

        /// <summary>
        /// Decode bytes to string
        /// </summary>
        /// <param name="value">Array of byte</param>
        /// <returns>String</returns>
        public string Decoding(byte[] value)
        {
            return System.Text.Encoding.UTF8.GetString(value);
        }
    }
}
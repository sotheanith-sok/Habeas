namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// Generic encoder and decoder interface
    /// </summary>
    /// <typeparam name="T">Type to convert from and to bytes</typeparam>
    public interface IEncoderDecoder<T>
    {
        byte[] Encoding(T value);
        T Decoding(byte[] value);
    }
}
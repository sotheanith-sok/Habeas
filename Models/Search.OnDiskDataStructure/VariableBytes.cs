using System;
using System.Collections.Generic;
using System.Linq;

namespace Search.OnDiskDataStructure
{
    public class VariableBytes
    {
        /// <summary>
        /// Compress list of bytes array to a bytes array
        /// </summary>
        /// <param name="values">List of bytes array</param>
        /// <returns></returns>
        public static byte[] Compress(List<byte[]> values)
        {
            int byteCount = 0;
            for (int i = 0; i < values.Count; i++)
            {
                //Convert byte[] array to 4 bytes
                byte[] b = values[i];
                byte[] temp = new byte[4];
                Array.Copy(b, temp, b.Length);

                //Convert to int and encode
                byte[] encoded = Encode(BitConverter.ToInt32(temp, 0));

                //Keep track of size of result bytes
                byteCount += encoded.Length;

                //Save result
                values[i] = encoded;
            }

            byte[] result = new byte[byteCount];
            int count = 0;
            foreach (byte[] b in values)
            {
                Array.Copy(b, 0, result, count, b.Length);
                count += b.Length;
            }
            return result;
        }

        /// <summary>
        /// Decompress byte array to list of bytes
        /// </summary>
        /// <param name="values">bytes array</param>
        /// <returns></returns>
        public static List<byte[]> DecompressToBytes(byte[] values)
        {
            List<byte[]> result = new List<byte[]>();
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < values.Length; i++)
            {
                byte b = values[i];
                bytes.Add(b);
                if ((b & (1 << 7)) == 128)
                {
                    result.Add(BitConverter.GetBytes(Decode(bytes.ToArray())));
                    bytes.Clear();
                }
            }
            return result;
        }

        /// <summary>
        /// Compress list of integers to bytes array
        /// </summary>
        /// <param name="values">List of integers</param>
        /// <returns></returns>
        public static byte[] Compress(List<int> values)
        {
            List<byte[]> bytes = new List<byte[]>();
            int byteCount = 0;
            for (int i = 0; i < values.Count; i++)
            {
                //Convert to int and encode
                byte[] encoded = Encode(values[i]);

                //Keep track of size of result bytes
                byteCount += encoded.Length;

                //Save result
                bytes.Add(encoded);
            }

            byte[] result = new byte[byteCount];
            int count = 0;
            foreach (byte[] b in bytes)
            {
                Array.Copy(b, 0, result, count, b.Length);
                count += b.Length;
            }
            return result;
        }

        /// <summary>
        /// Decompress bytes array to list of integers
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<int> DecompressToInts(byte[] values)
        {
            List<int> result = new List<int>();
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < values.Length; i++)
            {
                byte b = values[i];
                bytes.Add(b);
                if ((b & (1 << 7)) == 128)
                {
                    result.Add(Decode(bytes.ToArray()));
                    bytes.Clear();
                }
            }
            return result;
        }


        /// <summary>
        /// Converts a positive integer into a varialbe-byte-encoded byte array
        /// </summary>
        /// <example>
        /// 777 (binary: 1100001001)
        /// VB code: 0000 0110  1000 1001 (06 89)
        /// </example>
        public static byte[] Encode(int value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("value", value, "value must be 0 or greater");
            }
            else if (value == 0)
            {
                return new byte[] { 0x80 };
            }

            byte[] bytes = new byte[5];

            uint number = (uint)value;
            byte part;  //to store a stop bit and 7 bits
            bool stopBit = true;
            int i = 0;

            while (number != 0)
            {
                //Take the least significant 7 bits
                part = (byte)(0x7F & number);  //01111111
                                               //Add stop bit to MSB (0: continue reading, 1: stop reading)
                if (stopBit)
                {
                    part |= 0x80;     //10000000, set MSB as 1
                }
                else
                {
                    part &= 0x7F;     //01111111, set MSB as 0
                }
                //Add the encoded byte to byte groups
                bytes[i++] = part;

                //Change stopBit to false
                stopBit = false;
                //Shift right the number by 7 bits
                number >>= 7;
            }

            // Reverse the order of bytes
            Array.Resize(ref bytes, i);
            Array.Reverse(bytes);

            return bytes;
        }



        /// <summary>
        /// Converts variable-byte-encoded number to 32bit integer
        /// </summary>
        /// <param name="encoded">encoded bytes that represent an integer</param>
        /// <returns>decoded 32bit integer</returns>
        public static int Decode(byte[] encoded)
        {
            int count = encoded.Count() - 1;
            int num = 0;
            foreach (byte b in encoded)
            {
                // take least 7 bits in b and push it to the end of num
                num |= ((b & 127) << (7 * count));
                count--;
            }

            return (int)num;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;

namespace Search.OnDiskDataStructure
{
    
    /// <summary>
    /// VariableBytes represents multiple variable-byte-encoded numbers in byte[].
    /// </summary>
    public class VariableBytes
    {

        /// <summary>
        /// Converts a positive integer into a varialbe-byte-encoded byte array
        /// </summary>
        /// <example>
        /// 777 (binary: 1100001001)
        /// VB code: 0000 0110  1000 1001 (06 89)
        /// </example>
        public static byte[] Encode(int value)
        {
            if (value < 0) {
                throw new ArgumentOutOfRangeException("value", value, "value must be 0 or greater");
            } else if (value == 0) {
                return new byte[]{0x80};
            }

            byte[] bytes = new byte[5];
            
            uint number = (uint) value;
            byte part;  //to store a stop bit and 7 bits
            bool stopBit = true;
            int i = 0;
            
            while(number != 0)
            {
                //Take the least significant 7 bits
                part = (byte) (0x7F & number);  //01111111
                //Add stop bit to MSB (0: continue reading, 1: stop reading)
                if (stopBit) {
                    part |= 0x80;     //10000000, set MSB as 1
                } else {
                    part &= 0x7F;     //01111111, set MSB as 0
                }
                //Add the encoded byte to byte groups
                bytes[i++] = part;

                //Change stopBit to false
                stopBit = false;
                //Shift right the number by 7 bits
                number >>= 7;
            }

            //Reverse the order of bytes
            Array.Resize(ref bytes, i);
            Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// Converts a list of integer to a byte stream
        /// </summary>
        /// <param name="values">integers</param>
        /// <returns>encoded byte stream</returns>
        public static byte[] Encode(List<int> values)
        {
            byte[] byteStream = new byte[values.Count*5];

            int i=0;
            foreach(int val in values)
            {
                byte[] encoded = Encode(val);
                encoded.CopyTo(byteStream, i);
                i += encoded.Length;
            }

            Array.Resize(ref byteStream, i);
            return byteStream;
        }

        /// <summary>
        /// Converts variable-byte-encoded number to 32bit integer
        /// </summary>
        /// <param name="encoded">encoded bytes that represent an integer</param>
        /// <returns>decoded 32bit integer</returns>
        public static int Decode(byte[] encoded)
        {
            uint num = 0;
            foreach(byte b in encoded)
            {
                //take least 7 bits in b and push it to the end of num
                num |= (byte)(b & 0x7F);
                //check the stop bit
                if ( CheckStopBit(b) ) { break; }
                //shift 7bits to the left
                num <<= 7;
            }
            return (int) num;
        }

        /// <summary>
        /// Check if the stop bit (the most significant bit) is true (1)
        /// </summary>
        /// <param name="b">byte to check</param>
        /// <returns>true if stop bit is true</returns>
        public static bool CheckStopBit(byte b)
        {
            return ((b | 0x7F) == 0xFF);
        }

       

        public class EncodedByteStream
        {
            private byte[] value;   //bytes of multiple varialbe-byte-encoded numbers
            public int Pos {get; set;}  //indicator of where a partial byte array that represents a number starts

            public EncodedByteStream(byte[] value)
            {
                this.value = value;
                Pos = 0;
            }

            /// <summary>
            /// Extracts the bytes for a varialbe-byte-encoded number at a time.
            /// And updates the starting position of the next variable-byte-encoded number within the entire bytes.
            /// </summary>
            /// <returns>a partial byte array that represents one number from multiple variable bytes</returns>
            public byte[] Extract()
            {
                int start = Pos;
                int length = 1;
                
                while(true)
                {
                    //Check if the stopbit(MSB) is 1
                    if( CheckStopBit(value[Pos]) ) {
                        Pos++;
                        break;
                    }
                    else {
                        length++;
                        Pos++;
                    }
                }

                byte[] partialArray = new byte[length];
                Array.Copy(value, start, partialArray, 0, length);
                return partialArray;
            }

            /// <summary>
            /// Skips bytes that represent a number
            /// </summary>
            public void Skip()
            {
                this.Extract();
            }

            /// <summary>
            /// Converts a partial byte array that represents one number into 32bit integer
            /// </summary>
            /// <returns></returns>
            public int ReadDecodedInt()
            {
                byte[] bytes = this.Extract();
                return Decode(bytes);
            }

        }
    }
}
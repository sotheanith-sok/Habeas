using System.Collections;
using System.Collections.Generic;
using System;
namespace Search.OnDiskDataStructure
{
    /// <summary>
    /// An implmenetation of variable bytes encoding to flatten list<byte[]> to byte[]
    /// </summary>
    public class Compressor
    {
        /// <summary>
        /// Convert from list of bools to bytes
        /// </summary>
        /// <param name="value">List of bools</param>
        /// <returns>bytes array</returns>
        private static byte[] ToBytes(List<bool> value)
        {
            BitArray a = new BitArray(value.ToArray());
            byte[] bytes = new byte[a.Length / 8 + (a.Length % 8 == 0 ? 0 : 1)];
            a.CopyTo(bytes, 0);

            return bytes;
        }

        /// <summary>
        /// Compress List<byte[]> to byte []
        /// </summary>
        /// <param name="values">List of byte array</param>
        /// <returns>Comprressed bytes</returns>
        public static byte[] Compress(List<byte[]> values)
        {
            
            List<bool> finalResult = new List<bool>();
            foreach (byte[] bytes in values)
            {
                //Convert byte to bool array
                BitArray bitArray = new BitArray(bytes);

                //Convert array to list of easier minipulation
                List<bool> boolList = new List<bool>();
                for (int i = 0; i < bitArray.Count; i++)
                {
                    boolList.Add(bitArray.Get(i));
                }
                //Switch from big to small eden
                boolList.Reverse();

                //Remove leading 0 but keep the last 0 if the number is 0;
                while (boolList[0] == false)
                {
                    if (boolList.Count == 1)
                    {
                        break;
                    }
                    boolList.RemoveAt(0);
                }
                //Pad so bytes is divisible by 7
                while (boolList.Count % 7 != 0)
                {
                    boolList.Insert(0, false);
                }

                //Group into 7
                List<List<bool>> temp = new List<List<bool>>();
                for (int i = 0; i < boolList.Count; i += 7)
                {
                    temp.Add(boolList.GetRange(i, 7));
                }

                //Inject 1 or 0 to front
                for (int i = 0; i < temp.Count - 1; i++)
                {
                    temp[i].Insert(0, false);
                }
                temp[temp.Count - 1].Insert(0, true);

                //Merge again
                for (int i = 0; i < temp.Count; i++)
                {
                    finalResult.AddRange(temp[i]);
                }
            }
            return ToBytes(finalResult);
        }

        /// <summary>
        /// Decompress byte[] to List<byte[]>
        /// </summary>
        /// <param name="values">Compressed bytes</param>
        /// <returns>List of byte[]. (Need 0 padding to convert to other datatype)</returns>
        public static List<byte[]> Decompress(byte[] values)
        {
            BitArray bits = new BitArray(values);

            List<bool> bools = new List<bool> { };

            for (int i = 0; i < bits.Count; i++)
            {
                bools.Add(bits.Get(i));
            }

            List<List<bool>> boolList = new List<List<bool>>();

            //Chuck into byte
            for (int i = 0; i < bools.Count; i += 8)
            {
                boolList.Add(bools.GetRange(i, 8));
            }


            List<List<bool>> realValue = new List<List<bool>>();
            int k = 0;

            //Merage base on the prefix
            while (boolList.Count > 0)
            {
                List<bool> temp = boolList[0];
                boolList.RemoveAt(0);

                if (temp[0] == false)
                {
                    if (realValue.Count <= k)
                    {
                        realValue.Add(new List<bool>());
                    }
                    realValue[k].AddRange(temp.GetRange(1, 7));
                }
                else
                {
                    if (realValue.Count <= k)
                    {
                        realValue.Add(new List<bool>());
                    }
                    realValue[k].AddRange(temp.GetRange(1, 7));
                    k++;
                }
            }

            //Convet list of bool to bytes[]
            List<byte[]> finalResult = new List<byte[]>();
            foreach (List<bool> b in realValue)
            {
                b.Reverse();
                finalResult.Add(ToBytes(b));
            }

            return finalResult;
        }

    }
}

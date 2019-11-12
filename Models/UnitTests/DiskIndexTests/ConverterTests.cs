using Xunit;
using FluentAssertions;
using Search.OnDiskDataStructure;
using System.Collections.Generic;
using System;
using System.IO;
using Search.Index;
namespace UnitTests.OnDiskDataStructureTests
{
    public class ConverterTests
    {

        /// <summary>
        /// A little test for compression
        /// </summary>
        [Fact]
        public void TestConversion()
        {
            //Compress
            List<byte[]> testDdata = new List<byte[]>();
            testDdata.Add(BitConverter.GetBytes(159357));
            testDdata.Add(BitConverter.GetBytes(456852));
            testDdata.Add(BitConverter.GetBytes(957351));
            byte[] rawBytes = Compressor.Compress(testDdata);

            //Decompress
            List<byte[]> result = Compressor.Decompress(rawBytes);

            //Need bigger array between int expect 4 bytes
            byte[] ik = new byte[4];
            result[0].CopyTo(ik, 0);
            BitConverter.ToInt32(ik, 0).Should().Equals(159357);
            ik = new byte[4];
            result[1].CopyTo(ik, 0);
            BitConverter.ToInt32(ik, 0).Should().Equals(456852);
            ik = new byte[4];
            result[2].CopyTo(ik, 0);
            BitConverter.ToInt32(ik, 0).Should().Equals(957351);
        }

        /// <summary>
        /// Little test for compression/decompression of posting list
        /// </summary>
        [Fact]
        public void TestPostingConversion(){
            Posting p = new Posting(1, new List<int>{1,10,100,51651});
            Posting p2 = new Posting(2, new List<int>{1,10,100});
            Posting p3 = new Posting(3, new List<int>{1,10,100,100});
            Posting p4 = new Posting(4, new List<int>{1,10,100});
            List<Posting> list = new List<Posting>{p,p2,p3,p4};

            PostingListEncoderDecoder a = new PostingListEncoderDecoder();

            list = a.Decoding(a.Encoding(list));
            list.Should().BeEquivalentTo(new List<Posting>{p,p2,p3,p4});

        }
    }

}

using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Search.OnDiskDataStructure;
using System;
using Search.Index;

namespace UnitTests.OnDiskDataStructureTests
{
    public class InvertedIndexListEncodingTests
    {
        [Fact]
        public void EncodingTest()
        {
            //Arrange
            List<MaxPriorityQueue.InvertedIndex> postings = new List<MaxPriorityQueue.InvertedIndex>();
            postings.Add(new MaxPriorityQueue.InvertedIndex(8, 5));
            postings.Add(new MaxPriorityQueue.InvertedIndex(10, 5));
            //expected written index:
            //<df, doc1, tf,  doc2gap, tf,  ...>
            //2, 8, 3, 0, 5, 5, 2, 2, 10, 5

            //Act
            var encoder = new InvertedIndexEncoderDecoder();
            var actual = encoder.Encoding(postings);
            var decoded = encoder.Decoding(actual);

            Console.WriteLine("------------");
            Console.WriteLine(decoded[0].GetDocumentId());
            Console.WriteLine(decoded[0].GetTermFreq());
            Console.WriteLine(decoded[1].GetDocumentId());
            Console.WriteLine(decoded[1].GetTermFreq());
            Console.WriteLine("------------");

            //Assert
            //                           2,    5,   8,    0,    10
            var expected = new byte[]{0x82,  0x85, 0x88, 0x80, 0x8A};
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DecodingTest()
        {
            //Arrange
            var bytes = new byte[]{0x82, 0x85, 0x88, 0x80, 0x8A};
            List<MaxPriorityQueue.InvertedIndex> expected = new List<MaxPriorityQueue.InvertedIndex>();
            expected.Add(new MaxPriorityQueue.InvertedIndex(8, 5));
            expected.Add(new MaxPriorityQueue.InvertedIndex(10,5));

            //Act
            var encoder = new InvertedIndexEncoderDecoder();
            var actual = encoder.Decoding(bytes);

            //Assert
            (actual[0].GetDocumentId()).Should().Be(expected[0].GetDocumentId());

        }
        
    }
}
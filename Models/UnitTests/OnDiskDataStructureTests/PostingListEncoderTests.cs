using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Search.OnDiskDataStructure;
using System;
using Search.Index;

namespace UnitTests.OnDiskDataStructureTests
{
    public class PostingListEncodingTests
    {
        [Fact]
        public void EncodingTest()
        {
            //Arrange
            List<Posting> postings = new List<Posting>();
            postings.Add(new Posting(8, new List<int>(){0,5,10}));
            postings.Add(new Posting(10, new List<int>(){10,15}));
            //expected written index:
            //<df, doc1, tf, p1, p2Gaps, doc2Gap, tf, p1, p2Gaps, ...>
            //2, 8, 3, 0, 5, 5, 2, 2, 10, 5

            //Act
            var encoder = new PostingListEncoderDecoder();
            var actual = encoder.Encoding(postings);
        
            //Assert
            //                           2,    8,    3,    0,    5,    5,    2,    2,   10,    5
            var expected = new byte[]{0x82, 0x88, 0x83, 0x80, 0x85, 0x85, 0x82, 0x82, 0x8A, 0x85};
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DecodingTest()
        {
            //Arrange
            var bytes = new byte[]{0x82, 0x88, 0x83, 0x80, 0x85, 0x85, 0x82, 0x82, 0x8A, 0x85};
            List<Posting> expected = new List<Posting>();
            expected.Add(new Posting(8, new List<int>(){0,5,10}));
            expected.Add(new Posting(10, new List<int>(){10,15}));

            //Act
            var encoder = new PostingListEncoderDecoder();
            var actual = encoder.Decoding(bytes);

            //Assert
            actual.Should().BeEquivalentTo(expected);

        }
        
    }
}
using System.Collections.Generic;
using FluentAssertions;
using Search.Index;
using Xunit;

namespace UnitTests.DiskIndexTests
{
    [Collection("FileIORelated")]
    public class DiskPositionalIndexTests
    {
        private static string dirPath = "../../../Models/UnitTests/testCorpus/testCorpusBasic/index/";
        
        // [Fact]
        [Theory]
        // [InlineData(0, "(3,[6])")]  //'a' starts at 0    //NOTE: error at BinaryReader accessing again
        [InlineData(53*4, "(3,[1])")]  //'love' starts at (53 * 4byte)
        // [InlineData(4*4, "(1,[3]), (4,[7])")]  //'full' starts at (4*4)
        public void ReadPostingsTest(int startByte, string expectedPostings)
        {
            //Arrange
            DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
            IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
            //Act
            IList<Posting> actual = diskIndex.ReadPostings(startByte);
            //Assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Theory]
        // [InlineData(4*4, "(1,[3]), (4,[7])")]   //'full'
        [InlineData(11*4, "(0,[0,1]), (2,[0,2,3])")]   //'hello'
        public void ReadPostingsTestWithGap(int startByte, string expectedPostings)
        {
            //Arrange
            DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
            IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
            //Act
            IList<Posting> actual = diskIndex.ReadPostings(startByte);
            actual.Should().BeEquivalentTo(expected);


        }


    }
}
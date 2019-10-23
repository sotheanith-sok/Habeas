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
        
        [Theory]
        [InlineData(4*4, "(1,[]), (4,[])")]          //'full'   //test docId gap
        public void ReadPostingsTest(int startByte, string expectedPostings)
        {
            //Arrange
            DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
            IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
            //Act
            IList<Posting> actual = diskIndex.ReadPostings(startByte, false);
            //Assert
            actual.Should().BeEquivalentTo(expected);

            diskIndex.Dispose();
        }

        [Theory]
        [InlineData(4*4,  "(1,[3]), (4,[7])")]          //'full'   //test docId gap
        [InlineData(11*4, "(0,[0,1]), (2,[0,2,3])")]   //'hello'  //test position gap
        public void ReadPostingsTestWithPositions(int startByte, string expectedPostings)
        {
            //Arrange
            DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
            IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
            //Act
            IList<Posting> actual = diskIndex.ReadPostings(startByte, true);
            //Assert
            actual.Should().BeEquivalentTo(expected);

            diskIndex.Dispose();
        }



    }
}
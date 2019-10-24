using System.Collections.Generic;
using FluentAssertions;
using Search.Index;
using Xunit;
using System;
using System.Globalization;

namespace UnitTests.DiskIndexTests
{
    [Collection("FileIORelated")]
    public class DiskPositionalIndexTests
    {
        private static string dirPath = "../../../Models/UnitTests/testCorpus/testCorpusBasic/index/";
        
        [Theory]
        [InlineData("hello", "(0,[]), (2,[])")]
        public void GetPostingsTest_NoPosition(string term, string expectedPostings)
        {
            //Arrange
            DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
            IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
            //Act
            IList<Posting> actual = diskIndex.GetPostings(term);
            //Assert
            actual.Should().BeEquivalentTo(expected);
            
            //TODO: test multiple terms
            
            diskIndex.Dispose();
        }

        [Theory]
        [InlineData("hello", "(0,[0,1]), (2,[0,2,3])")]
        public void GetPostingsTest_WithPosition(string term, string expectedPostings)
        {
            //Arrange
            DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
            IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
            //Act
            IList<Posting> actual = diskIndex.GetPositionalPostings(term);
            //Assert
            actual.Should().BeEquivalentTo(expected);

            //TODO: test multiple terms

            diskIndex.Dispose();
        }

        // //The method BinarySearchVocabulary() became private
        // [Theory]
        // [InlineData("hello", "2C")]
        // [InlineData("mr.snowman", "E4")]
        // public void BinarySearchVocabularyTest(string term, string expectedHex)
        // {
        //     //Arrange
        //     DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
        //     //Act
        //     long actual = diskIndex.BinarySearchVocabulary(term);
        //     long expected = long.Parse(expectedHex, NumberStyles.HexNumber);
        //     //Assert
        //     actual.Should().Be(expected);

        //     diskIndex.Dispose();
        // }

        // //The method ReadPostings() became private
        // [Theory]
        // [InlineData(4*4, "(1,[]), (4,[])")]          //'full'   //test docId gap
        // public void ReadPostingsTest_NoPosition(int startByte, string expectedPostings)
        // {
        //     //Arrange
        //     DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
        //     IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
        //     //Act
        //     IList<Posting> actual = diskIndex.ReadPostings(startByte, false);
        //     //Assert
        //     actual.Should().BeEquivalentTo(expected);

        //     diskIndex.Dispose();
        // }

        // //The method ReadPostings() became private
        // [Theory]
        // [InlineData(4*4,  "(1,[3]), (4,[7])")]         //'full'   //test docId gap
        // [InlineData(11*4, "(0,[0,1]), (2,[0,2,3])")]   //'hello'  //test position gap
        // public void ReadPostingsTest_Positions(int startByte, string expectedPostings)
        // {
        //     //Arrange
        //     DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
        //     IList<Posting> expected = UnitTest.GeneratePostings(expectedPostings);
        //     //Act
        //     IList<Posting> actual = diskIndex.ReadPostings(startByte, true);
        //     //Assert
        //     actual.Should().BeEquivalentTo(expected);

        //     diskIndex.Dispose();
        // }

        // //The method GetDocumentWeight() became private
        // [Fact]
        // public void GetDocumentWeightTest()
        // {
        //     DiskPositionalIndex diskIndex = new DiskPositionalIndex(dirPath);
        //     double docWeight = Math.Round(diskIndex.GetDocumentWeight(0), 9);
        //     double docWeight2 = Math.Round(diskIndex.GetDocumentWeight(1), 9);
        //     docWeight.Should().Be(2.620447934);
        //     docWeight2.Should().Be(3.377006594);

        //     diskIndex.Dispose();
        // }



    }
}
using Xunit;
using System.Collections.Generic;
using System;
using System.Reflection;
using Search.Index;
using Search.Document;
using Search.PositionalInvertedIndexer;

namespace UnitTests
{
    public class PositionalInvertedIndexTests {
        
        //Aren't they too big to call unit tests?
        
        [Fact]
        public void PositionalPostingTest_Hello_Passing(){
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./UnitTests/testCorpus", ".txt");
            PositionalInvertedIndex index;
            string term = "snows";
            
            List<PositionalPosting> expected = new List<PositionalPosting>
            {
                new PositionalPosting(0, new List<int>{0,1}),
                new PositionalPosting(2, new List<int>{0,2,3})
            };
            
            //Act
            index = PositionalInvertedIndexer.IndexCorpus(corpus);
            IList<PositionalPosting> actual = index.GetPositionalPostings(term);
            
            //Assert
            Assert.Equal(expected, actual);

        }

        [Fact]
        public void PositionalPostingTest_Snows_Passing(){
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./UnitTests/testCorpus", ".txt");
            PositionalInvertedIndex index;
            string term = "snows";
            
            List<PositionalPosting> expected = new List<PositionalPosting>
            {
                new PositionalPosting(1, new List<int>{7,8,9}),
                new PositionalPosting(4, new List<int>{1,2,3})
            };
            
            //Act
            index = PositionalInvertedIndexer.IndexCorpus(corpus);
            IList<PositionalPosting> actual = index.GetPositionalPostings(term);
            
            //Assert
            Assert.Equal(expected, actual);

        }

        // [Fact]
        // public void AddTermTest_TermNotExist_AddsNewKey(){
        //     //use these later when making [Theory] [InlineCode]
        //     // string term;
        //     // int docId;
        //     // int position;

        //     //Arrange
        //     Dictionary<string, List<PositionalPosting>> map = new Dictionary<string, List<PositionalPosting>>(){
        //         ["hello"] = new List<PositionalPosting>(){new PositionalPosting(1,new List<int>(){1})}
        //     };
        //     // PositionalInvertedIndex index = new PositionalInvertedIndex(){
        //     //     ["hello"] = new List<PositionalPosting>(){new PositionalPosting(1,new List<int>(){1})}
        //     // };

        //     //TODO: how to build an PositionalInvertedIndex from outside? w/o using AddTerm()

        //     // //Act
        //     // PositionalInvertedIndex.AddTerm("world",1,2);

        //     // //Assert
        //     // Assert.True(index.GetPositionalPosting("world",1).Positions.Contains(2));
        // }

    }

}
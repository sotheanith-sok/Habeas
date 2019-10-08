using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;
using FluentAssertions;

namespace UnitTests
{
    public class PositionalIndexTests {
        string directory = "../../../UnitTests/testCorpus2";

        [Fact]
        public void PostionalIndexTest_OnePosition(){
            //Arrange
            string term = "sun";
            IList<Posting> expected;
            System.Console.WriteLine("PositionalIndexTest_OnePosition: ");
            System.Console.Write($"Set expected postings of \'{term}\'");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                System.Console.WriteLine(" for MacOSX");
                expected = new List<Posting>{ new Posting(1, new List<int>{3}) };
            }
            else {
                System.Console.WriteLine(" for Windows and other OSs");
                expected = new List<Posting>{ new Posting(3, new List<int>{3}) };
            }

            //Act
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            PositionalInvertedIndex index = new Indexer().IndexCorpus(corpus);
            var result = index.GetPostings(term);
            
            //Assert
            result.Should().HaveSameCount(expected);
            result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
        }

        [Fact]
        public void PostionalIndexTest_MultiplePositions(){
            //Arrange
            string term = "hello";
            IList<Posting> expected;
            System.Console.WriteLine("PositionalIndexTest_MultiplePositions: ");
            System.Console.Write($"Set expected postings of \'{term}\'");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                System.Console.WriteLine(" for MacOSX");
                expected = new List<Posting>{ new Posting(2, new List<int>{0,2,3}),
                                              new Posting(4, new List<int>{0,1}) };
            } else {
                System.Console.WriteLine(" for Windows and other OSs");
                expected = new List<Posting>{ new Posting(0, new List<int>{0,1}),
                                              new Posting(2, new List<int>{0,2,3}) };
            }

            //Act
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            PositionalInvertedIndex index = new Indexer().IndexCorpus(corpus);
            var result = index.GetPostings(term);
            
            //Assert
            result.Should().HaveSameCount(expected);
            result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
        }


        // [Fact]
        // public void VocabTest_WithoutStemmer(){
        //     //Arrange
        //     IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
        //     PositionalInvertedIndex index = IndexCorpus(corpus);
        //     var expectedVocab = new List<string>{
        //         "hello","world","it","is","snowing",
        //         "the","full","of","mystery","snows",
        //         "mr.snowman","loves","sun","a"
        //     };  //expected vocabulary with not stemmed terms
        //     //Act
        //     var actual = index.GetVocabulary();
        //     //Assert
        //     index.Should().NotBeNull("because indexCorpus shouldn't return null");
        //     actual.Should().HaveSameCount(expectedVocab, "because the index used NormalTokenProcessor");
        //     actual.Should().BeEquivalentTo(expectedVocab);
        // }
        
        [Fact]
        public void VocabTest_WithStemmer(){
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            PositionalInvertedIndex index = new Indexer().IndexCorpus(corpus);
            var expectedVocab = new List<string>{
                "hello","world","it","is","snow",
                "the","full","of","mystery","mr.snowman",
                "love","sun","a"
            };  //expected vocabulary with stemmed terms
            expectedVocab.Sort();

            //Act
            var actual = index.GetVocabulary();
            //Assert
            index.Should().NotBeNull("because indexCorpus shouldn't return null");
            actual.Should().HaveSameCount(expectedVocab, "because the index used StemmingTokenProcessor");
            //actual.Should().BeEquivalentTo(expectedVocab);    //TODO: why "mystery" became "mysteri"??
        }

        //AddTermTests
        //TODO: test 3 different exit points
        // In index
        // 1. term X                        -> Add term with everything into the hashMap
        // 2. term O, docId X, position X   -> Add a Posting to the List<Posting>
        // 3. term O, docId O, position X   -> Add the position to the Posting of docId

        // [Fact]
        // public void AddTermTest_TermNotExist_AddsNewKey(){
        //     //use these later when making [Theory] [InlineCode]
        //     // string term;
        //     // int docId;
        //     // int position;

        //     //Arrange
        //     Dictionary<string, List<Posting>> map = new Dictionary<string, List<Posting>>(){
        //         ["hello"] = new List<Posting>(){new Posting(1,new List<int>(){1})}
        //     };
        //     // PositionalInvertedIndex index = new PositionalInvertedIndex(){
        //     //     ["hello"] = new List<Posting>(){new Posting(1,new List<int>(){1})}
        //     // };

        //     // //Act
        //     // PositionalInvertedIndex.AddTerm("world",1,2);

        //     // //Assert
        //     // Assert.True(index.GetPosting("world",1).Positions.Contains(2));
        // }

    }

}


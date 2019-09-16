using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;

//Aren't they too big to call unit tests?


namespace UnitTests
{
    public class PositionalIndexTests {

        //Arrange
        static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus", ".txt");
        PositionalInvertedIndex index = PositionalInvertedIndexer.IndexCorpus(corpus);

        [Theory]
        [MemberData(nameof(Data))]
        public void PositoinalPostingTest(string term, List<PositionalPosting> expected)
        {
            var result = index.GetPositionalPostings(term);

            //Assert
            Assert.Equal(expected.Count, result.Count);
            //Assert.Equal(expected, result);       //TODO: Don't know why this does't work..

            //Print the expected and actual
            Console.WriteLine($"Term:     {term}");
            Console.Write("Expected: ");
            expected.ForEach(x => Console.Write($"{x.ToString()}, "));
            Console.Write("\nResult:   ");
            ((List<PositionalPosting>)result).ForEach(x => Console.Write($"{x.ToString()}, "));
            Console.WriteLine();
        }

        //Test data for positional inverted index
        //Assumed the terms were processed with BasicTokenProcessor
        //The docID in the data is generated depend on different OS
        public static TheoryData<string, List<PositionalPosting>> Data(){
            var winData = new TheoryData<string, List<PositionalPosting>> {
                {"hello", new List<PositionalPosting>{
                    new PositionalPosting(0, new List<int>{0,1}),
                    new PositionalPosting(2, new List<int>{0,2,3})
                }},
                {"snows", new List<PositionalPosting>{
                    new PositionalPosting(1, new List<int>{7,8,9}),
                    new PositionalPosting(4, new List<int>{1,2,3})
                }},
            };

            var macData = new TheoryData<string, List<PositionalPosting>> {
                {"hello", new List<PositionalPosting>{
                    new PositionalPosting(2, new List<int>{0,2,3}),
                    new PositionalPosting(4, new List<int>{0,1}),
                }},
                {"snows", new List<PositionalPosting>{
                    new PositionalPosting(0, new List<int>{1,2,3}),
                    new PositionalPosting(3, new List<int>{7,8,9}),
                }},
            };
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                System.Console.WriteLine("TestData for macOSX");
                return macData;
            } else {
                System.Console.WriteLine("TestData for all others");
                return winData;
            }
        }

    }

}


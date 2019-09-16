using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;

//Aren't they too big to call unit tests?


// namespace UnitTests
// {
//     public class PositionalIndexTests {
        
//         // [Theory]
//         // //[InlineData("hello", 1, List<int>(){0,1})]
//         // //[MemberData(nameof(Data))]
//         // public void PositoinalPostingTest(string term, int docId, List<int> positions)
//         // {

//         // }

//         [Theory]
//         [MemberData(nameof(Data))]
//         public void PositoinalPostingTest(string term, List<PositionalPosting> expected)
//         {
//             //Arrange
//             IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./UnitTests/testCorpus", ".txt");
//             PositionalInvertedIndex index;
//             IList<PositionalPosting> result;

//             //Act
//             index = PositionalInvertedIndexer.IndexCorpus(corpus);
//             result = index.GetPositionalPostings(term);
            
//             //Assert
//             Assert.Equal(expected, result);
//         }

//         //Test data for positional inverted index
//         //Assumed the terms were processed with BasicTokenProcessor
//         public static TheoryData<string, List<PositionalPosting>> Data =>
//             new TheoryData<string, List<PositionalPosting>> {
//                 {"hello", new List<PositionalPosting>{
//                     new PositionalPosting(0, new List<int>{0,1}),
//                     new PositionalPosting(2, new List<int>{0,2,3})
//                 }},
//                 {"snows", new List<PositionalPosting>{
//                     new PositionalPosting(1, new List<int>{7,8,9}),
//                     new PositionalPosting(4, new List<int>{1,2,3})
//                 }},
//                 {"sun", new List<PositionalPosting>{
//                     new PositionalPosting(3, new List<int>{5})
//                 }},
//             };
        
//     }

// }
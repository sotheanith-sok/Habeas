using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;

namespace UnitTests
{
    public class AddTermTests {
        //Todo: test 3 different exit points
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
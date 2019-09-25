using System.Collections.Generic;
using Cecs429.Search.Query;
using Search.Index;
using Xunit;

namespace UnitTests
{
    public class PhraseLiteralTests
    {
        [Fact]
        public void PositionalMergeTest() {
            //Arrange
            IList<Posting> first = new List<Posting>{
                new Posting(1, new List<int>{2,5,10}),
                new Posting(2, new List<int>{4,19}),
                new Posting(3, new List<int>{0,90}),
                new Posting(5, new List<int>{13,18})
            };
            IList<Posting> second = new List<Posting>{
                new Posting(1, new List<int>{1,4,11,20}),
                new Posting(3, new List<int>{1,91}),
                new Posting(5, new List<int>{12,20})
            };
            IList<Posting> expected = new List<Posting>{
                new Posting(1, new List<int>{10}),
                new Posting(3, new List<int>{0,90})
            };
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(first, second, 1);

            //Assert
            //TODO: Use Fluent Assertion to solve this issue.
            //Assert.Equal(expected, result);
            Assert.Equal(expected[0].ToString(), result[0].ToString());
            
        }

        [Fact]
        public void PositionalMergeTest_NoResult_ReturnsEmptyPostings(){
            IList<Posting> first = new List<Posting>{
                new Posting(1, new List<int>{2,5})
            };
            IList<Posting> second = new List<Posting>{
                new Posting(1, new List<int>{11,20})
            };
            IList<Posting> expected = new List<Posting>();
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(first, second, 1);

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void PositionalMergeTest_AllRelevant_ReturnsAllMatching(){
            IList<Posting> first = new List<Posting>{
                new Posting(1, new List<int>{0,5}),
                new Posting(2, new List<int>{10,20}),
                new Posting(4, new List<int>{1040,2800,9999})
            };
            IList<Posting> second = new List<Posting>{
                new Posting(1, new List<int>{1,6}),
                new Posting(2, new List<int>{11,21}),
                new Posting(4, new List<int>{1041,2801,10000})
            };
            IList<Posting> expected = first;
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(first, second, 1);

            //Assert
            //TODO: Use Fluent Assertion and edit below!
            Assert.Equal(expected[0].ToString(), result[0].ToString());
            Assert.Equal(expected[1].ToString(), result[1].ToString());
            Assert.Equal(expected[2].ToString(), result[2].ToString());
        }

        // /// <summary>
        // /// generate a posting list from a string
        // /// </summary>
        // /// <param name="postings">a string in (doc1,[pos1,pos2,pos3]),(doc2,[pos1,...]) form</param>
        // /// <returns></returns>
        // public IList<Posting> GeneratePostings(string str) {
        //     IList<Posting> postingList = new List<Posting>();
        //     //untanggle the string
        //     int docId = 0;
        //     str.Substring(str.IndexOf('('));
            
        //     //Add it to postingList
        //     return postingList;
        // }
    }
}
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
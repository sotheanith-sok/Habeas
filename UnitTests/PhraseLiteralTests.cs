using System.Collections.Generic;
using Cecs429.Search.Query;
using Search.Index;
using Xunit;
using FluentAssertions;

namespace UnitTests
{
    public class PhraseLiteralTests
    {
        [Fact]
        public void PositionalMergeTest_WithTwoPostingLists_PhraseExist_ReturnsFirstPos()
        {
            //Arrange
            IList<Posting> first = new List<Posting>{
                new Posting(1, new List<int>{0})        //angels
            };
            IList<Posting> second = new List<Posting>{
                new Posting(1, new List<int>{1})        //baseball
            };
            
            var expected = new List<Posting>{
                new Posting(1, new List<int>{0})        //starting position of "angels baseball"
            };

            //Act
            var result = (new PhraseLiteral("ttt")).PositionalMerge(first,second);

            //Assert
            result.Should().BeOfType(typeof(List<Posting>));
            result.Should().BeEquivalentTo(expected);   
        }

        [Fact]
        public void PositionalMergeTest_WithTwoPostingLists_PhraseNotExist_ReturnsEmpty()
        {
            //Arrange
            IList<Posting> first = new List<Posting>{
                new Posting(1, new List<int>{0})        //angels
            };
            IList<Posting> second = new List<Posting>{
                new Posting(1, new List<int>{10})        //baseball
            };

            //Act
            var result = (new PhraseLiteral("ttt")).PositionalMerge(first,second);

            //Assert
            result.Should().BeOfType(typeof(List<Posting>));
            result.Should().BeEmpty("because there's no match set");
        }

    //PositionalMerge with Recursion (List of posting lists and The last one as an input)
        [Fact]
        public void PositionalMergeTest_WithListAndOne_PhraseExist_ReturnsFirstPos()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>();
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{0})            //kentucky
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{1})            //fried
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{2})            //chicken
            });

            IList<Posting> last = new List<Posting>{        //sims
                new Posting(1, new List<int>{3})
            };

            IList<Posting> expected = new List<Posting>{
                new Posting(1, new List<int>{0})            //starting position of "kentucky fried chicken sims"
            };

            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list, last, list.Count);

            //Assert
            result.Should().BeEquivalentTo(expected);
            
        }

        [Fact]
        public void PositionalMergeTest_WithListAndOne_PhraseNotExist_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>();
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{0})
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{100})
            });
            IList<Posting> last = new List<Posting>{
                new Posting(1, new List<int>{1})
            };
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list, last, list.Count);

            //Assert
            result.Should().BeEmpty("because there's no match set");
        }

        [Fact]
        public void PositionalMergeTest_WithListAndOne_PartialMatch_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>();
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{0})        //kentucky
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{1})        //fried
            });
            IList<Posting> last = new List<Posting>{
                new Posting(1, new List<int>{3})        //chicken   -off
            };
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list, last, list.Count);

            //Assert
            result.Should().BeEmpty("because there's only some part of postings match not the whole");
        }

    //PositionalMerge with Recursion (List of posting lists as an input)
        [Fact]
        public void PositionalMergeTest_WithList_PhraseExist_ReturnsFirstPos()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>();
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{0})            //kentucky
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{1})            //fried
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{2})            //chicken
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{3})            //sims
            });

            IList<Posting> expected = new List<Posting>{
                new Posting(1, new List<int>{0})            //starting position of "kentucky fried chicken sims"
            };

            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list);

            //Assert
            result.Should().BeEquivalentTo(expected, "because PositionalMerge should work with list of all posting lists");
            
        }

        [Fact]
        public void PositionalMergeTest_WithList_PhraseNotExist_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>();
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{0})            //kentucky
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{100})            //fried
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{2})            //chicken
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{3})            //sims
            });

            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list);

            //Assert
            result.Should().BeEmpty("because there's no match set");
            
        }

        [Fact]
        public void PositionalMergeTest_WithList_PartialMatch_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>();
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{0})            //kentucky
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{1})            //fried
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{2})            //chicken
            });
            list.Add(new List<Posting>{
                new Posting(1, new List<int>{300})          //sims
            });

            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list);

            //Assert
            result.Should().BeEmpty("because there's only some part of postings match not the whole");
            
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
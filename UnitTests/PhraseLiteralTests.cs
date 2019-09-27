using System.Collections.Generic;
using Cecs429.Search.Query;
using Search.Index;
using Xunit;
using FluentAssertions;
using System.Linq;
using System;

namespace UnitTests
{
    public class PhraseLiteralTests
    {
        [Fact]
        public void PositionalMergeTest_WithTwoPostingLists_PhraseExist_ReturnsFirstPos()
        {
            //Arrange
            IList<Posting> first = GeneratePostings("(1,[0,5,10,30])"); //angels
            IList<Posting> second = GeneratePostings("(1,[1,11,31])");  //baseball
            
            IList<Posting> expected = GeneratePostings("(1,[0,10,30])"); //angels //starting position of "angels baseball"
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(first,second);

            //Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PositionalMergeTest_WithTwoPostingLists_PhraseNotExist_ReturnsEmpty()
        {
            //Arrange
            IList<Posting> first = GeneratePostings("(1,[0])");   //angels
            IList<Posting> second = GeneratePostings("(1,[10])"); //baseball -off

            //Act
            var result = (new PhraseLiteral("ttt")).PositionalMerge(first,second);

            //Assert
            result.Should().BeOfType(typeof(List<Posting>));
            result.Should().BeEmpty("because there's no match set");
        }

    //PositionalMerge with Recursion (List of posting lists and The last one as an input)
        [Fact]
        public void PositionalMergeTest_WithRecursion_PhraseExist_ReturnsFirstPos()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0])"),        //kentucky
                GeneratePostings("(1,[1])"),        //fried
                GeneratePostings("(1,[2])")         //chicken
            };
            IList<Posting> last = GeneratePostings("(1,[3])");    //sims -off
            IList<Posting> expected = GeneratePostings("(1,[0])");  //starting position of "kentucky fried chicken sims"
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list, last, list.Count);

            //Assert
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PositionalMergeTest_WithRecursion_PhraseNotExist_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0])"),        //kentucky
                GeneratePostings("(1,[100])")         //fried -off
            };
            IList<Posting> last = GeneratePostings("(1,[1])");    //chicken
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list, last, list.Count);

            //Assert
            result.Should().BeEmpty("because there's no match set");
        }

        [Fact]
        public void PositionalMergeTest_WithRecursion_PartialMatch_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0])"),        //kentucky
                GeneratePostings("(1,[1])")         //fried
            };
            IList<Posting> last = GeneratePostings("(1,[100])");    //chicken -off
            
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
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0]), (2,[5,10,44,100])"),     //kentucky
                GeneratePostings("(1,[1]), (2,[11,50,101])"),       //fried
                GeneratePostings("(1,[2]), (2,[0,12,102,200])"),    //chicken
                GeneratePostings("(1,[3]), (2,[3,13,103])")         //sims
            };

            IList<Posting> expected = GeneratePostings("(1,[0]), (2,[10,100])"); //starting position of "kentucky fried chicken sims"
            
            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list);

            //Assert
            result.Should().BeEquivalentTo(expected, "because PositionalMerge should work with list of all posting lists");
            
        }

        [Fact]
        public void PositionalMergeTest_WithList_PhraseNotExist_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0])"),        //kentucky
                GeneratePostings("(1,[100])"),      //fried
                GeneratePostings("(1,[2])"),        //chicken
                GeneratePostings("(1,[300])")       //sims
            };

            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list);

            //Assert
            result.Should().BeEmpty("because there's no match set");
            
        }

        [Fact]
        public void PositionalMergeTest_WithList_PartialMatch_ReturnsEmpty()
        {
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0])"),        //kentucky
                GeneratePostings("(1,[1])"),        //fried
                GeneratePostings("(1,[2])"),        //chicken
                GeneratePostings("(1,[300])")       //sims
            };

            //Act
            IList<Posting> result = (new PhraseLiteral("ttt")).PositionalMerge(list);

            //Assert
            result.Should().BeEmpty("because there's only some part of postings match not the whole");
            
        }

        /// <summary>
        /// Generates a list of postings from a string
        /// It should follow the form of "(d1,[p1,p2,p3]), (d2,[p1,...]), (..)" form.
        /// </summary>
        /// <param name="str">a string to generate postings</param>
        /// <returns></returns>
        public static IList<Posting> GeneratePostings(string str) {
            if( !str.StartsWith('(') || !str.EndsWith(')') ) {
                Console.WriteLine("not a correct string to generate postings");
                return null;
            }
            Console.WriteLine($"Generating postings from string \'{str}\'");
            IList<Posting> postingList = new List<Posting>();
            //untanggle the string of postings
            str = str.TrimStart('(').TrimEnd(')');
            List<string> str_postings = str.Split("), (").ToList();
            foreach (string str_p in str_postings)
            {
                //Console.Write($"str_p: {str_p}\t->\t");
                int docId = Int32.Parse( str_p.Substring(0,str_p.IndexOf(',')) );
                //untanggle the string of positions
                string trimedPositions = str_p.Substring(str_p.IndexOf('[')).TrimStart('[').TrimEnd(']');
                List<string> str_positions = trimedPositions.Split(',').ToList();
                List<int> positions = new List<int>();
                foreach(string str_posit in str_positions) {
                    positions.Add(Int32.Parse(str_posit));
                }
                //make a posting with docId and positions
                Posting posting = new Posting(docId, positions);
                postingList.Add(posting);
                //Console.WriteLine($"object: {posting.ToString()}");
            }
            
            Console.WriteLine($"Generated: {postingList.Count} postings.");
            return postingList;
        }

    }
}
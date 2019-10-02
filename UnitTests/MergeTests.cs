using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Search.Index;
using Search.Query;
using Xunit;

namespace UnitTests
{
    public class MergeTests
    {

    // Tests for AndlMerge --------------------------------------------------------------
        [Fact]
        public void AndMergeTest_AllOverlap_ReturnsOneOfThePostingLists(){
            //Test with two
            IList<Posting> first = GeneratePostings("(1,[0,5]), (2,[10,99])");
            IList<Posting> second = GeneratePostings("(1,[2,9]), (2,[77])");

            IList<Posting> result = Merge.AndMerge(first, second);
            result.Should().HaveCount(first.Count, "because all overlaps");
            
            //Test with list
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0,5]), (2,[1])"),
                GeneratePostings("(1,[3,6]), (2,[7])"),
                GeneratePostings("(1,[4]), (2,[3,6])"),
                GeneratePostings("(1,[2,8]), (2,[9])")
            };
            result = Merge.AndMerge(list);
            result.Should().HaveCount(2, "because all in the list overlaps");
        }

        [Fact]
        public void AndMergeTest_NoOverlap_ReturnsEmpty(){
            //Test with two
            IList<Posting> first = GeneratePostings("(1,[0,5]), (5,[10])");
            IList<Posting> second = GeneratePostings("(2,[2,9]), (7,[77])");

            IList<Posting> result = Merge.AndMerge(first, second);
            result.Should().BeEmpty("because nothing overlaps between the two");
            result.Should().HaveCount(0, "because nothing overlaps between the two");

            //Test with list
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0,5]), (2,[4])"),
                GeneratePostings("(2,[6]), (3,[7])"),
                GeneratePostings("(1,[2]), (3,[1,9])"),
                GeneratePostings("(3,[8]), (4,[6])")
            };
            result = Merge.AndMerge(list);
            result.Should().BeEmpty("because nothing in the list overlaps");
        }

        [Fact]
        public void AndMergeTest_SomeOverlap_ReturnsMatchingPostings(){
            //Test with two
            IList<Posting> first = GeneratePostings("(1,[0,5]), (2,[10,99])");
            IList<Posting> second = GeneratePostings("(1,[2,9]), (3,[77])");

            IList<Posting> result = Merge.AndMerge(first, second);
            result.Should().HaveCount(1, "because only 1 posting overlap");

            //Test with list
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0,5]), (2,[8])"),
                GeneratePostings("(2,[1,9])"),
                GeneratePostings("(1,[3]), (2,[4]), (3,[9])"),
                GeneratePostings("(2,[7]), (3,[7])")
            };
            result = Merge.AndMerge(list);
            result.Should().HaveCount(1, "because only 1 posting overlap");
        }

    // Tests for OrMerge --------------------------------------------------------------
        [Fact]
        public void OrMergeTest_AllOverlap_ReturnsOneOfThePostingLists(){
            //Test with two
            IList<Posting> first = GeneratePostings("(1,[0,5]), (2,[10,99])");
            IList<Posting> second = GeneratePostings("(1,[2,9]), (2,[77])");
            
            IList<Posting> result = Merge.OrMerge(first, second);
            result.Should().HaveCount(first.Count, "because all overlaps");

            //Test with list
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0,5]), (2,[1])"),
                GeneratePostings("(1,[3,6]), (2,[7])"),
                GeneratePostings("(1,[4]), (2,[3,6])"),
                GeneratePostings("(1,[2,8]), (2,[9])")
            };
            result = Merge.OrMerge(list);
            result.Should().HaveCount(2, "because all in the list overlaps");
        }

        [Fact]
        public void OrMergeTest_NoOverlap_ReturnsAllPostings(){
            //Test with two
            IList<Posting> first = GeneratePostings("(1,[0,5]), (5,[10])");
            IList<Posting> second = GeneratePostings("(2,[2,9]), (7,[77])");
            IList<Posting> result = Merge.OrMerge(first, second);
            result.Should().HaveCount(4, "because nothing overlaps between the two");

            //Test with list
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0,5])"),
                GeneratePostings("(2,[10])"),
                GeneratePostings("(4,[30]), (5,[99])"),
                GeneratePostings("(3,[7]), (6,[77])")
            };
            result = Merge.OrMerge(list);
            result.Should().HaveCount(6, "because nothing in the list overlaps");
        }

        [Fact]
        public void OrMergeTest_SomeOverlap_ReturnsPostings(){
            //NOTE: OrMerge don't need to have the correct positions?
            //Test with two posting lists
            //Arrange
            IList<Posting> first = GeneratePostings("(1,[0,5]), (2,[10,99])");
            IList<Posting> second = GeneratePostings("(1,[2,9]), (3,[77])");
            // IList<Posting> expected = GeneratePostings("(1,[0,2,5,9]), (2,[10,99]), (3,[77])");
            //Act
            IList<Posting> result = Merge.OrMerge(first, second);
            //Assert
            result.Should().HaveCount(3);
            // result.Should().HaveSameCount(expected);
            // result.Should().BeEquivalentTo(expected, "because some posting overlaps and some not.");

            //Test with list of posting lists
            //Arrange
            List<IList<Posting>> list = new List<IList<Posting>>{
                GeneratePostings("(1,[0,5])"),
                GeneratePostings("(2,[10])"),
                GeneratePostings("(1,[30]), (2,[99])"),
                GeneratePostings("(1,[7]), (3,[77])")
            };
            // IList<Posting> expected = GeneratePostings("(1,[0,5,77,30]), (2,[10,99]), (3,[77])");
            //Act
            result = Merge.OrMerge(list);
            //Assert
            result.Should().HaveCount(3);
            // result.Should().HaveSameCount(expected);
            // result.Should().BeEquivalentTo(expected);
        }

    // Tests for PostionalMerge --------------------------------------------------------------
        [Fact]
        public void PositionalMergeTest_WithTwoPostingLists_PhraseExist_ReturnsFirstPos()
        {
            //Arrange
            IList<Posting> first = GeneratePostings("(1,[0,5,10,30])"); //angels
            IList<Posting> second = GeneratePostings("(1,[1,11,31])");  //baseball
            
            IList<Posting> expected = GeneratePostings("(1,[0,10,30])"); //angels //starting position of "angels baseball"
            
            //Act
            IList<Posting> result = Merge.PositionalMerge(first,second);

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
            var result = Merge.PositionalMerge(first,second);

            //Assert
            result.Should().BeOfType(typeof(List<Posting>));
            result.Should().BeEmpty("because there's no match set");
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
            IList<Posting> result = Merge.PositionalMerge(list);

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
            IList<Posting> result = Merge.PositionalMerge(list);

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
            IList<Posting> result = Merge.PositionalMerge(list);

            //Assert
            result.Should().BeEmpty("because there's only some part of postings match not the whole");
            
        }



        /// <summary>
        /// Generates a list of postings from a string
        /// It should follow the form of "(d1,[p1,p2,p3]), (d2,[p1,...]), (..)" form.
        /// </summary>
        /// <param name="str">a string to generate postings</param>
        /// <returns></returns>
        public IList<Posting> GeneratePostings(string str) {
            if( !str.StartsWith('(') || !str.EndsWith(')') ) {
                Console.WriteLine("not a correct string to generate postings");
                return null;
            }
            // Console.WriteLine($"Generating postings from string \'{str}\'");
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
            
            // Console.WriteLine($"Generated: {postingList.Count} postings.");
            return postingList;
        }
    }
}
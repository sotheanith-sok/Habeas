using System;
using Xunit;
using Cecs429.Search.Query;
using System.Collections.Generic;

namespace UnitTests
{
    public class BooleanQueryParserTests
    {
        private BooleanQueryParser BooleanQueryParser;
        public BooleanQueryParserTests()
        {
            BooleanQueryParser = new BooleanQueryParser();
        }

        private class Literal
        {
            /// <summary>
            /// The bounds of the literal within the original query.
            /// </summary>
            public StringBounds Bounds { get; }

            /// <summary>
            /// A parsed IQueryComponent that corresponds to the literal.
            /// </summary>
            public IQueryComponent LiteralComponent { get; }

            public Literal(StringBounds bounds, IQueryComponent literal)
            {
                Bounds = bounds;
                LiteralComponent = literal;
            }
        }

        private struct StringBounds
        {
            /// <summary>
            /// The starting index of the string portion.
            /// </summary>
            public int Start { get; }
            /// <summary>
            /// The length of the string portion.
            /// </summary>
            public int Length { get; }


            public StringBounds(int start, int length)
            {
                Start = start;
                Length = length;
            }
        }
        private StringBounds FindNextSubquery(String query, int startIndex)
        {
            int lengthOut;

            // Find the start of the next subquery by skipping spaces and + signs.
            char test = query[startIndex];
            while (test == ' ' || test == '+')
            {
                test = query[++startIndex];
            }

            // Find the end of the next subquery.
            int nextPlus = query.IndexOf('+', startIndex + 1);

            if (nextPlus < 0)
            {
                // If there is no other + sign, then this is the final subquery in the
                // query string.
                lengthOut = query.Length - startIndex;
            }
            else
            {
                // If there is another + sign, then the length of this subquery goes up
                // to the next + sign.

                // Move nextPlus backwards until finding a non-space non-plus character.
                test = query[nextPlus];
                while (test == ' ' || test == '+')
                {
                    test = query[--nextPlus];
                }

                lengthOut = 1 + nextPlus - startIndex;
            }

            // startIndex and lengthOut give the bounds of the subquery.
            return new StringBounds(startIndex, lengthOut);
        }

        /// <summary>
        /// Locates and returns the next literal from the given subquery string.
        /// </summary>
        private Literal FindNextLiteral(String subquery, int startIndex)
        {

            int subLength = subquery.Length;
            int lengthOut;

            // Skip past white space.
            while (subquery[startIndex] == ' ')
            {
                ++startIndex;
            }

            //If a non-white space character starts with '"'
            if (subquery[startIndex] == '"')
            {
                //move the startIndex up by one, we don't care about the '"' character
                startIndex++;
                // Locate the next space to find the closing '"' character.
                int nextSpace = subquery.IndexOf('"', startIndex);
                // Assuming some loathsome Boetian forgets to end the quote
                if (nextSpace < 0)
                {
                    // We'll just assume that they meant the rest of the subquery
                    lengthOut = subquery.Length - startIndex;
                }
                else
                {
                    //Otherwise, the position of the next '"' character is the end of the phrase
                    lengthOut = nextSpace - startIndex - 1;
                }
                //create the PhraseLiteral
                return new Literal(
              // startIndex and lengthOut identify the bounds of the literal
              new StringBounds(startIndex, lengthOut),
              // we assume this is a single term literal... for now
              new PhraseLiteral(subquery.Substring(startIndex, lengthOut)));
            }
            // JESSE'S EASTER EGG:
            // POETRY INTERLUDE!
            // Do not stand at my grave and weep,
            // I am no there, I do not sleep.
            // I am a thousand winds that blow.
            // I am the diamond glint on snow.
            // I am the the sunlight on ripened grain.
            // I am the gentle autumn rain.
            // When you awake amid the morning hush,
            // I am the swift uplifting rush,
            // Of quiet birds in circling flight.
            // I am the soft star that shine at night.
            // Do not stand at my grave and cry,
            //I am not there, I did not die.
            else
            {
                // Locate the next space to find the end of this literal.
                int nextSpace = subquery.IndexOf(' ', startIndex);
                if (nextSpace < 0)
                {
                    // No more literals in this subquery.
                    lengthOut = subLength - startIndex;
                }
                else
                {
                    lengthOut = nextSpace - startIndex;
                }

                //otherwise, stem
                // This is a term literal containing a single term.
                return new Literal(
                 // startIndex and lengthOut identify the bounds of the literal
                 new StringBounds(startIndex, lengthOut),
                 // we assume this is a single term literal... for now
                 new TermLiteral(subquery.Substring(startIndex, lengthOut)));
            }
        }

        /// <summary>
        /// Test BooleanQueryParser's capacity to handle a phrase without closing quotation marks
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Fact]
        
        
        public void TestHandlingQuoteError()
        {

            //    [InlineData("\"I hate forgetting to close my parenthesis", ]
        // [InlineData("\"", Literal testLiteral2 = new Literal())]
            //Assert.Equal(expected, this.FindNextLiteral(token, 0));

        }
        //result.Should().BeEquivalentTo(expected, "reasons");


    }

}

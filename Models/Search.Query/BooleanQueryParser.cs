using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Search.Text;
using Search.Index;

namespace Search.Query
{
    /// <summary>
    /// Parses Boolean queries to produce an object representation that can be executed to find documents
    /// satisfying the query. Does not handle phrase queries, NOT queries, NEAR queries, 
    /// or wildcard queries... yet.
    /// </summary>
    /// <example>
    /// Index myIndex = ...;
    /// BooleanQueryParser p = new BooleanQueryParser();
    /// IQueryComponent c = p.ParseQuery("white whale + captain ahab");
    /// var postings = c.GetPostings(myIndex);
    /// </example>
    public class BooleanQueryParser
    {
        /// <summary>
        /// Identifies a portion of a string with a starting index and a length.
        /// </summary>
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

        /// <summary>
        /// Encapsulates a parsed literal from a query, including the literal's bounds within the query
        /// and an IQueryComponent for that portion of the query.
        /// </summary>
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

        /// <summary>
        /// Parses a Boolean search query to produce an IQueryComponent for that query.
        /// </summary>
        /// <param name="query">query to be parsed to a query component</param>
        /// <returns>a query component</returns>
        public IQueryComponent ParseQuery(String query)
        {
            // General routine: scan the query to identify a literal, and put that literal into a list.
            //	Repeat until a + or the end of the query is encountered; build an AND query with each
            //	of the literals found. Repeat the scan-and-build-AND-query phase for each segment of the
            // query separated by + signs. In the end, build a single OR query that composes all of the built
            // AND subqueries.
            if(query == null) { return null; }
            query = query.TrimStart(' ').TrimEnd(' ');
            if(query == "" || query == null) { return null; }

            int start = 0;
            var allSubqueries = new List<IQueryComponent>();
            do
            {
                // Identify the next subquery: a portion of the query up to the next + sign.
                StringBounds nextSubquery = FindNextSubquery(query, start);
                // Extract the identified subquery into its own string.
                String subquery = query.Substring(nextSubquery.Start, nextSubquery.Length);
                int subStart = 0;

                // Store all the individual components of this subquery.
                var subqueryLiterals = new List<IQueryComponent>();

                do
                {
                    // Extract the next literal from the subquery.
                    Literal lit = FindNextLiteral(subquery, subStart);

                    // Add the literal component to the conjunctive list.
                    subqueryLiterals.Add(lit.LiteralComponent);

                    // Set the next index to start searching for a literal.
                    subStart = lit.Bounds.Start + lit.Bounds.Length;

                } while (subStart < subquery.Length);

                // After processing all literals, we are left with a list of query components that we are
                // ANDing together, and must fold that list into the final OR list of components.

                // If there was only one literal in the subquery, we don't need to AND it with anything --
                // its component can go straight into the list.
                if (subqueryLiterals.Count == 1)
                {
                    allSubqueries.Add(subqueryLiterals[0]);
                }
                else
                {
                    // With more than one literal, we must wrap them in an AndQuery component.
                    allSubqueries.Add(new AndQuery(subqueryLiterals));
                }
                start = nextSubquery.Start + nextSubquery.Length;
            } while (start < query.Length);

            // After processing all subqueries, we either have a single component or multiple components
            // that must be combined with an OrQuery.
            if (allSubqueries.Count == 1)
            {
                return allSubqueries[0];
            }
            else if (allSubqueries.Count > 1)
            {
                return new OrQuery(allSubqueries);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Locates the start index and length of the next subquery in the given query string
        /// starting at the given index.
        /// </summary>
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

            // Capture a Phrase query
            if (subquery[startIndex] == '"')
            {
                //move the startIndex up by one, we don't care about the '"' character
                startIndex++;
                // Locate the next space to find the closing '"' character.
                int nextSpace = subquery.IndexOf('"', startIndex);
                // Assuming some loathsome Boetian forgets to end the quote
                if (nextSpace < 0) {
                    // We'll just assume that they meant the rest of the subquery
                    lengthOut = subquery.Length - startIndex;
                }
                else {
                    //Otherwise, the position of the next '"' character is the end of the phrase
                    lengthOut = nextSpace - startIndex + 1;
                }
                string phrase = subquery.Substring(startIndex, lengthOut);
                phrase = phrase.TrimStart('"').TrimEnd('"');

                //create the PhraseLiteral
                return new Literal(
                    new StringBounds(startIndex, lengthOut),
                    new PhraseLiteral(phrase)
                );
            }
            // Capture a Near query
            else if (subquery[startIndex] == '[')
            {
                startIndex++;
                // Find closing bracket ']'
                int closingIndex = subquery.IndexOf(']', startIndex);
                // Limit near query parts
                if(closingIndex < 0) {  //not have closing bracket
                    lengthOut = subquery.Length - startIndex;
                } else {                //have closing bracket
                    lengthOut = closingIndex - startIndex + 1;
                }
                // Extract the near query substring
                string near = subquery.Substring(startIndex, lengthOut);
                near = near.TrimStart('[').TrimEnd(']').ToLower();
                Console.WriteLine(near);

                //Handle exceptions with Regex
                // 1) no "near/"
                //Regex rgx_near = new Regex(@"\s<near/>\d+\s");
                if (near.Contains(" near/") == false) {
                    //create TermLiteral instead with the first term appears
                    string term1 = near.Substring(0, near.IndexOf(' '));
                    Console.WriteLine($"not a proper near query, searching for the first term \"{term1}\"");
                    return new Literal(
                        new StringBounds(startIndex, lengthOut),
                        new TermLiteral(term1)
                    );
                }
                // TODO: 2) no k value
                // 3) more than one word on either side

                // Split the string into 3 parts
                string[] parts = near.Split(' ');
                // Get first and second term
                string first = parts[0];
                string second = parts[2];
                // Detect "near/" and get k value
                string k_str = parts[1].Substring("near/".Length);
                int k = Int32.Parse(k_str);

                // create NearLiteral(term1, k, term2)
                return new Literal(
                    new StringBounds(startIndex, lengthOut),
                    new NearLiteral(first,k,second)
                );
            }
            // Capture a Wildcard or Single query otherwise
            else
            {
                // Locate the next space to find the end of this literal.
                int nextSpace = subquery.IndexOf(' ', startIndex);
                if (nextSpace < 0) { // No more literals in this subquery.
                    lengthOut = subLength - startIndex;
                }
                else {
                    lengthOut = nextSpace - startIndex;
                }

                //Capture a Wildcard query
                if (subquery.Substring(startIndex, lengthOut).Contains("*"))
                {
                    //create WildcardLiteral
                    return new Literal(
                        new StringBounds(startIndex, lengthOut),
                        new WildcardLiteral(subquery.Substring(startIndex, lengthOut), Indexer.kGram)
                    );
                }

                //If it's not captured at any of the above, it's a Single query
                //create TermLiteral
                return new Literal(
                    new StringBounds(startIndex, lengthOut),
                    new TermLiteral(subquery.Substring(startIndex, lengthOut))
                );
            }
            
        }

    }
}

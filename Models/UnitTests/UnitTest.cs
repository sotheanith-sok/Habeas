
using System;
using System.Collections.Generic;
using System.Linq;
using Search.Index;

namespace UnitTests
{
    /// <summary>
    /// This class contains methods to help other tests
    /// </summary>
    public class UnitTest
    {

        /// <summary>
        /// Generates a list of postings from a string
        /// It should follow the form of "(d1,[p1,p2,p3]), (d2,[p1,...]), (..)" form.
        /// </summary>
        /// <param name="str">a string to generate postings</param>
        /// <returns></returns>
        public static IList<Posting> GeneratePostings(string str)
        {
            if (!str.StartsWith('(') || !str.EndsWith(')'))
            {
                Console.WriteLine("GeneratePostings(): Not a correct string to generate postings");
                return null;
            }
            // Console.WriteLine($"Generating postings from string \'{str}\'");
            IList<Posting> postingList = new List<Posting>();
            //untanggle the string of postings
            str = str.TrimStart('(').TrimEnd(')');
            List<string> str_postings = str.Split("), (").ToList();
            foreach (string str_p in str_postings)
            {
                int docId = Int32.Parse(str_p.Substring(0, str_p.IndexOf(',')));
                //untanggle the string of positions
                string trimedPositions = str_p.Substring(str_p.IndexOf('[')).TrimStart('[').TrimEnd(']');
                List<string> str_positions = trimedPositions.Split(',').ToList();
                List<int> positions = new List<int>();
                foreach (string str_posit in str_positions)
                {
                    positions.Add(Int32.Parse(str_posit));
                }
                //make a posting with docId and positions
                Posting posting = new Posting(docId, positions);
                postingList.Add(posting);
            }

            // Console.WriteLine($"Generated: {postingList.Count} postings.");
            return postingList;
        }

        /// <summary>
        /// Print the posting list
        /// </summary>
        /// <param name="postings">postings to print</param>
        public static void PrintPostingResult(IList<Posting> postings)
        {
            string str_postings = StringifyPostings(postings);
            Console.WriteLine($"{postings.Count} postings: {str_postings}");
        }

        /// <summary>
        /// Convert a posting list to a string
        /// </summary>
        public static string StringifyPostings(IList<Posting> postings)
        {
            string str_postings = "";
            foreach (Posting p in postings)
            {
                str_postings = str_postings + p.ToString() + "  ";
            }
            return str_postings;
        }
    }
}
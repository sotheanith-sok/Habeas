using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Search.Text;
using Search.Index;
using System.Linq;

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
    public class SpellingChecker
    {

        /// <summary>
        /// Parses a search query to produce an IQueryComponent for Ranked Retrieval.
        /// <param name="query">query to be parsed to a query component</param>
        /// <returns>a query component</returns>
        public List<String> CheckSpelling(String query)
        {

            List<string> terms = query.Split(' ');
            terms.Remove("AND");
            terms.Remove("and");
            List<List<string>> processedTerms = new List<List<string>>();

            ITokenProcessor processor = new StemmingTokenProcesor();

            foreach (string term in terms)
            {
                if (term.Contains('*'))
                {
                    terms.Remove(term);
                }
                else
                {
                    processedTerms.Add(processor.ProcessToken(term));
                }
            }

            List<string> poorlySpelledWords = new List<string>();
            List<string> finalTerms = new List<string>();
            foreach (List<string> term in processedTerms)
            {
                foreach (string independentTerm in term)
                {
                    finalTerms.Add(independentTerm);
                }
            }

            foreach (string term in finalTerms){
                TermLiteral a = new TermLiteral(term);
                IList<Posting> termsPostings = a.GetPostings();
                if (termsPostings.Count < 6){
                    poorlySpelledWords.Add(term);
                }
            }

            // foreach term in poorly spelled words
            // send that term into the spell checker
            // reconstruct the proper query in correctlySpelledQuery

            return correctlySpelledQuery;
        }

    }
}
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
    public class RankedRetrievalParser
    {

        /// <summary>
        /// Parses a search query to produce an IQueryComponent for Ranked Retrieval.
        /// <param name="query">query to be parsed to a query component</param>
        /// <returns>a query component</returns>
        public List<string> ParseQuery(string query)
        {


            if (query.Contains('*'))
            {
                return new List<string>();
            }

            string[] terms = query.Split(' ');
            List<List<string>> processedTerms = new List<List<string>>();

            ITokenProcessor processor = new StemmingTokenProcesor();

            foreach (string term in terms)
            {
                processedTerms.Add(processor.ProcessToken(term));
            }

            List<string> finalTerms = new List<string>();
            foreach (List<string> term in processedTerms)
            {
                foreach (string independentTerm in term)
                {
                    finalTerms.Add(independentTerm);
                }
            }
            
            return finalTerms;
        }

    }
}
using System.Collections.Generic;
using Search.Index;
using Search.Text;

namespace Search.Query
{

    /// <summary>
    /// Represents a term literal consisting of one word.
	/// TermLiteral stores a not processed token
    /// </summary>
	public class TermLiteral : IQueryComponent
    {
        public string Term { get; }

        public TermLiteral(string term)
        {
            Term = term;
        }

        /// <summary>
        /// Get Postings
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="processor">Tokene processor</param>
        /// <returns></returns>
        public IList<Posting> GetPostings(IIndex index, ITokenProcessor processor)
        {
            //Process the term
            List<string> processedTerms = processor.ProcessToken(Term);
            //Gets a or-merged posting list from all results of multiple terms from index...
            return index.GetPostings(processedTerms);
        }

        /// <summary>
        /// Convert this query componenet to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Term;
        }
    }
}

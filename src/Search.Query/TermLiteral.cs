using System.Collections.Generic;
using Search.Index;
using Search.Text;

namespace Search.Query {

    /// <summary>
    /// Represents a term literal consisting of one word.
	/// TermLiteral stores a not processed token
    /// </summary>
	public class TermLiteral : IQueryComponent {
		public string Term { get; }

		public TermLiteral(string term) {
			Term = term;
		}

		public IList<Posting> GetPostings(IIndex index, ITokenProcessor processor) {
			//Process the term
			List<string> processedTerms = processor.ProcessToken(Term);
			//Gets a or-merged posting list from all results of multiple terms from index...
			return index.GetPostings(processedTerms);
		}

		public override string ToString() {
			return Term;
		}
	}
}

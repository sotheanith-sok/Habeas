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
			//TODO: TermLiteral better to get one processed token than a list of token...
			
			//string processedTerm = processor.ProcessToken(Term);
			return index.GetPostings(Term);
		}

		public override string ToString() {
			return Term;
		}
	}
}

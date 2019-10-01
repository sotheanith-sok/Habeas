using System.Collections.Generic;
using Search.Index;

namespace Search.Query {
	public class TermLiteral : IQueryComponent {
		public string Term { get; }

		public TermLiteral(string term) {
			Term = term;
		}

		public IList<Posting> GetPostings(IIndex index) {
			return index.GetPostings(Term);
		}

		public override string ToString() {
			return Term;
		}
	}
}

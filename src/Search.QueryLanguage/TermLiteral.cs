using System;
using System.Collections.Generic;
using System.Text;
using Search.Index;

namespace Cecs429.Search.Query {
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

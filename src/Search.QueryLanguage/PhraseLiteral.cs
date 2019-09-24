using System;
using System.Collections.Generic;
using System.Text;
using Search.Index;

namespace Cecs429.Search.Query {
	/// <summary>
	/// Represents a phrase literal consisting of one or more terms that must occur in sequence.
	/// </summary>
	public class PhraseLiteral : IQueryComponent {
		// The list of individual terms in the phrase.
		private List<string> mTerms = new List<string>();

		/// <summary>
		/// Constructs a PhraseLiteral with the given individual phrase terms.
		/// </summary>
		public PhraseLiteral(List<string> terms) {
			mTerms.AddRange(terms);
		}

		/// <summary>
		/// Constructs a PhraseLiteral given a string with one or more individual terms separated
		/// by spaces.
		/// </summary>
		public PhraseLiteral(string terms) {
			mTerms.AddRange(terms.Split(' '));
		}

		public IList<Posting> GetPostings(IIndex index) {
			throw new NotImplementedException();
			// TODO: program this method. Retrieve the postings for the individual terms in the phrase,
			// and positional merge them together.
		}

		public override string ToString() {
			return "\"" + string.Join(" ", mTerms) + "\"";
		}
	}
}

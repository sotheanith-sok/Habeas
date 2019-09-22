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
			// List<IList<Posting>> TermPostingsList = new List<IList<Posting>>();
			// //Retrieves the postings for the individual terms in the phrase
			// foreach(string s in mTerms){
			// 	TermLiteral TL = new TermLiteral(s);
			// 	IList<Posting> termPostings = TL.GetPostings(index);
			// 	TermPostingsList.Add(termPostings);
			// }
			// // and positional merge them together.
			throw new NotImplementedException();
		}

		public override string ToString() {
			return "\"" + string.Join(" ", mTerms) + "\"";
		}

		
	}
}

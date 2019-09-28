using System.Collections.Generic;
using Cecs429.Search.Query;
using Search.Index;

namespace Search.Query
{
    /// <summary>
    /// Represents a phrase literal consisting of one or more terms that must occur in sequence.
    /// </summary>
    public class PhraseLiteral : IQueryComponent {
		// The list of individual terms in the phrase.
		private List<string> mTerms = new List<string>();
		// public string Phrase => "\"" + string.Join(" ", mTerms) + "\"";

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
		public PhraseLiteral(string phrase) {
			//NOTE: Words in the phrase are not processed.
			//TODO: Needs to process it before dealing them like processed terms
			mTerms.AddRange(phrase.Split(' '));
		}

		public IList<Posting> GetPostings(IIndex index) {
			//A list of posting lists (postings for each term in the phrase)
			List<IList<Posting>> postingsList = new List<IList<Posting>>();
			//Retrieves the postings for the individual terms in the phrase
			foreach(string term in mTerms) {
				postingsList.Add(index.GetPostings(term));
			}
			//positional merge all posting lists
			return Merge.PositionalMerge(postingsList);
		}


		public override string ToString() {
			return "\"" + string.Join(" ", mTerms) + "\"";
		}
	}
}

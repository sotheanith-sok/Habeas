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
			//A list of posting lists (postings for each term in the phrase)
			List<IList<Posting>> postingsList = new List<IList<Posting>>();
			//Retrieves the postings for the individual terms in the phrase
			foreach(string term in mTerms) {
				postingsList.Add(index.GetPostings(term));
			}

			//positional merge all posting lists
			// return PositionalMerge(postingsList);
			throw new NotImplementedException();
		}

		public override string ToString() {
			return "\"" + string.Join(" ", mTerms) + "\"";
		}

		/// <summary>
		/// Merge posting lists of terms in a phrase into one list of postings
		/// based on their consecutive positions in a document.
		/// </summary>
		/// <param name="postingsList">a list of posting lists from multiple terms</param>
		/// <returns></returns>
		public IList<Posting> PositionalMerge(List<IList<Posting>> postingsList) {
			IList<Posting> first = postingsList[0];
			postingsList.RemoveAt(0);
			//positional merge them together.

			//PositionalMerge( 1st, 2nd, 1)
			//PositionalMerge( (1st+2nd), 3rd, 2)
			if (postingsList.Count <= 2) {}
			//TODO: implement the rest using recursive call

			return null;
		}

		/// <summary>
		/// Merge posting lists of two terms into one list of postings
		/// based on their positions in a document with an offset value.
		/// </summary>
		/// <param name="first">the first list of postings</param>
		/// <param name="second">the second list of postings</param>
		/// <param name="gap">the gap between the terms in the phrase. default: 1</param>
		/// <returns></returns>
		public IList<Posting> PositionalMerge(IList<Posting> first, IList<Posting> second, int gap = 1) {
			IList<Posting> newPostingList = new List<Posting>();
			
			int i=0;	//track docID in first postings
			int j=0;	//track docID in second postings
			//Iterate through all postings in the first and second.
			while ( (i<first.Count) && (j<second.Count) )
			{
				//Compare the document IDs
				if (first[i].DocumentId == second[j].DocumentId)
				{
					//then first[i] and second[j] are the candidates.
					int x=0;	//track the position in i doc
					int y=0;	//track the position in j doc (check if y is off by 'gap' from x)
					List<int> pp1 = first[i].Positions;
					List<int> pp2 = second[j].Positions;
					List<int> newPositions = new List<int>();
					//Iterate through all positions in the posting first[i] and second[j]
					while ( (x<pp1.Count) && (y<pp2.Count) )
					{
						//Compare the positions
						int difference = pp2[y] - pp1[x];
						if ( difference == gap ) {			//y is off by gap from x
							//Add to new positions
							newPositions.Add(pp1[x]);
							x++;
							y++;
						} else if ( difference > gap ) {	//y is too far from x
							x++;
						} else {							//y comes before x
							y++;
						}
					}
					//Add to the posting list
					if(newPositions.Count != 0){
						newPostingList.Add(new Posting(first[i].DocumentId, newPositions));
					}
					i++;
					j++;
				}
				else if ( first[i].DocumentId < second[j].DocumentId ) {
					i++;
				}
				else {
					j++;
				}
			}

			return newPostingList;
		}
	}
}

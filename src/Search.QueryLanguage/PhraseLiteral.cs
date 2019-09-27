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
			return PositionalMerge(postingsList);
		}

		/// <summary>
		/// Merge posting lists of terms in a phrase into one list of postings
		/// based on their consecutive positions in a document.
		/// </summary>
		/// <param name="list">a list of posting lists from multiple terms</param>
		/// <returns>a merged postings list</returns>
		public IList<Posting> PositionalMerge(List<IList<Posting>> list) {
			//handle exception
			if (list.Count == 0) {
				return new List<Posting>();
			} if (list.Count == 1) {
				return list[0];
			}

			int gap = list.Count-1;
			//extract last postings from the list
			IList<Posting> last = list[list.Count-1];
			list.RemoveAt(list.Count-1);
			//call recursive PositionalMerge()
			return PositionalMerge(list, last, gap);
		}

		/// <summary>
		/// Merge posting lists of terms into one list of postings
		/// based on their positions in a document with an offset value.
		/// </summary>
		/// <param name="list">the list of posting lists of terms that come first in a phrase</param>
		/// <param name="last">the last posting list of a term that comes after the terms in 'list'</param>
		/// <param name="gap">the gap between the terms in the phrase. default: 1</param>
		/// <returns>a merged postings list</returns>
		public IList<Posting> PositionalMerge(List<IList<Posting>> list, IList<Posting> last, int gap = 1) {
			if( list.Count == 1 ) {
				// base case
				return PositionalMerge(list[0], last, gap);
			}
			else {
				// recursive case
				IList<Posting> newLast = list[list.Count-1];
				list.RemoveAt(list.Count-1);
				return PositionalMerge( PositionalMerge(list, newLast, gap-1), last, gap);	//recursion
			}
		}

		/// <summary>
		/// Merge posting lists of two terms into one list of postings
		/// based on their positions in a document with an offset value.
		/// </summary>
		/// <param name="first">the first posting list</param>
		/// <param name="second">the second posting list</param>
		/// <param name="gap">the gap between the terms in the phrase. default: 1</param>
		/// <returns>a merged postings list</returns>
		public IList<Posting> PositionalMerge(IList<Posting> first, IList<Posting> second, int gap = 1) {
			IList<Posting> mergedList = new List<Posting>();
			
			int i=0;	//track docID in first postings
			int j=0;	//track docID in second postings
			//Iterate through all postings in the first and second.
			while ( (i<first.Count) && (j<second.Count) )
			{
				//Compare the document IDs
				if (first[i].DocumentId == second[j].DocumentId)
				{
					//then first[i] and second[j] be the candidates.
					int x=0;	//track the position in i doc
					int y=0;	//track the position in j doc (to check if y is off by 'gap' from x)
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
					if(newPositions.Count > 0){
						mergedList.Add(new Posting(first[i].DocumentId, newPositions));
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
			return mergedList;
		}


		public override string ToString() {
			return "\"" + string.Join(" ", mTerms) + "\"";
		}
	}
}

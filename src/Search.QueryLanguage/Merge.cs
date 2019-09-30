using System.Collections.Generic;
using Search.Index;

namespace Search.Query
{
    /// <summary>
    /// Merge class contains static merge methods for AndMerge, OrMerge, PositionalMerge
    /// </summary>
    public class Merge
    {
        //TODO: Move all 3 AndMerge() methods here

        //TODO: Move all 3 OrMerge() methods here


		/// <summary>
		/// Merge posting lists of terms in a phrase into one list of postings
		/// based on their consecutive positions in a document.
		/// </summary>
		/// <param name="list">a list of posting lists from multiple terms in a phrase</param>
		/// <returns>a merged postings list</returns>
		public static IList<Posting> PositionalMerge(List<IList<Posting>> list) {
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
		public static IList<Posting> PositionalMerge(List<IList<Posting>> list, IList<Posting> last, int gap = 1) {
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
		public static IList<Posting> PositionalMerge(IList<Posting> first, IList<Posting> second, int gap = 1) {
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

    }
}
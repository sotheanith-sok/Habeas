using System;
using System.Collections.Generic;
using Search.Index;

namespace Search.Query
{
    /// <summary>
    /// Merge class contains static merge methods for AndMerge, OrMerge, PositionalMerge
    /// </summary>
    public class Merge
    {
		// AndMerge ----------------------------------------------------------------------
		/// <summary>
        /// AND-Merge all posting lists from multiple components into one posting list
        /// </summary>
        /// <param name="list">a list of posting lists to be OR merged</param>
        /// <returns>an AND-merged posting list</returns>
        public static IList<Posting> AndMerge(List<IList<Posting>> list) {
            //exceptions
            if (list.Count == 0) { return new List<Posting>(); }
            if (list.Count == 1) { return list[0]; }
            if (list.Count == 2) { return AndMerge(list[0], list[1]); }

            //Sort the posting lists in ascending order to merge smaller lists first
            list.Sort( (a, b) => a.Count.CompareTo(b.Count) );

            //Prepare the posting lists to send to the recursive method
            //pop a list off of the end of list
            IList<Posting> biggest = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //Return the result of recursive merge
            return AndMerge(list, biggest);
        }

        /// <summary>
        /// AND-Merge a posting list with a list of posting lists with recursion
        /// </summary>
        /// <param name="list">a list of posting lists to be merged</param>
        /// <param name="biggest">the biggest posting list to be merged</param>
        /// <returns>an AND-merged posting list</returns>
        private static IList<Posting> AndMerge(List<IList<Posting>> list, IList<Posting> biggest)
        {
            //base case
            if (list.Count == 0) { return biggest; }    //all in the list have been merged
            if (list.Count == 1) { return AndMerge(list[0], biggest); }  //merge the two
            
            //recursive case
            //pop a list off of the end of listOfPList
            IList<Posting> secondBiggest = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //return the result of recursive merge
            return AndMerge( AndMerge(list, secondBiggest), biggest);
        }

        /// <summary>
        /// AND-Merge two posting lists into one
        /// </summary>
        /// <param name="first">first posting list to be merged</param>
        /// <param name="second">second posting list to be merged</param>
        /// <returns>an AND-merged posting list</returns>
        public static IList<Posting> AndMerge(IList<Posting> first, IList<Posting> second)
        {
            //a list to hold all the valid postings
            IList<Posting> finalList = new List<Posting>();
            //positions for comparing the values of the first and second list of postings
            int firstPosition = 0;
            int secondPosition = 0;

            //if one of the lists is empty, return an empty list
            if (first.Count <= 0 || second.Count <= 0)
            {
                return new List<Posting>();
            }
            
            //if both lists have postings in them...
            while (true)
            {
                //if the position has past the end of either list, the AND merge is complete  
                if (firstPosition > first.Count - 1 || secondPosition > second.Count - 1)
                {
                    return finalList;
                }
                //if the documentID of both postings are the same

                var firstItem = first[firstPosition];
                if (firstItem.DocumentId == second[secondPosition].DocumentId)
                {
                    //add that posting to the final list
                    finalList.Add(first[firstPosition]);
                    //move both positions up by one 
                    firstPosition++;
                    secondPosition++;
                }
                //if the documentID of the first is less than the DocumentID of the second
                else if (first[firstPosition].DocumentId < second[secondPosition].DocumentId)
                {
                    //move position of the first up by one
                    firstPosition++;
                }
                //if the documentID of the first is greater than the DocumentID of the second
                else if (first[firstPosition].DocumentId > second[secondPosition].DocumentId)
                {
                    //move position of the second up by one
                    secondPosition++;
                }
            }
        }


		// OrMerge ----------------------------------------------------------------------
        /// <summary>
        /// OR-Merge all posting lists from multiple components into one posting list
        /// </summary>
        /// <param name="list">a list of posting lists to be OR merged</param>
        /// <returns>an OR-merged posting list</returns>
        public static IList<Posting> OrMerge(List<IList<Posting>> list) {
            //exceptions
            if (list.Count == 0) { return new List<Posting>(); }
            if (list.Count == 1) { return list[0]; }
            if (list.Count == 2) { return OrMerge(list[0], list[1]); }

            //Sort the posting lists in ascending order to merge smaller lists first
            list.Sort( (a, b) => a.Count.CompareTo(b.Count) );
            IList<Posting> finalList = new List<Posting>(); 
            foreach(List<Posting> lp in list){
                finalList = OrMerge(lp, finalList);
            }
            return finalList;
        }

        /// <summary>
        /// OR-Merge a posting list with a list of posting lists with recursion
        /// </summary>
        /// <param name="list">a list of posting lists to be merged</param>
        /// <param name="biggest">the biggest posting list to be merged</param>
        /// <returns>an OR-merged posting list</returns>
        private static IList<Posting> OrMerge(List<IList<Posting>> list, IList<Posting> biggest)
        {
            //base case
            if (list.Count == 0) { return biggest; }    //all in the list have been merged
            if (list.Count == 1) { return OrMerge(list[0], biggest); }  //merge the two
            
            //recursive case
            //pop a list off of the end of listOfPList
            IList<Posting> secondBiggest = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //return the result of recursive merge
            return OrMerge( OrMerge(list, secondBiggest), biggest);
        }

        /// <summary>
        /// OR-Merge two posting lists into one
        /// </summary>
        /// <param name="first">first posting list to be merged</param>
        /// <param name="second">second posting list to be merged</param>
        /// <returns>an OR-merged posting list</returns>
        public static IList<Posting> OrMerge(IList<Posting> first, IList<Posting> second)
        {
            //a list to hold all the valid postings
            IList<Posting> finalList = new List<Posting>();
            //positions for comparing the values of the first and second list of postings
            int firstPosition = 0;
            int secondPosition = 0;
            
            //if both empty, return an empty list
            if (first.Count <= 0 && second.Count <= 0)
            {
                return new List<Posting>();
            }
            //if one of them is empty, return the other
            if (first.Count <= 0) { return second; }
            if (second.Count <= 0) { return first; }

            //if both lists have postings in them...
            while (true)
            {
                //if position one has past the end of list one... 
                if (firstPosition > first.Count - 1)
                {
                    //check to see if position two has past the end of list two
                    //if so, return whatever is in the final list
                    if (secondPosition > second.Count - 1) { return finalList; }
                    //otherwise, if there are some elements still in the second list
                    else
                    {
                        //iterate through the second list and add all the remaining elements to the final list
                        for (int i = secondPosition; i < second.Count; i++)
                        {
                            finalList.Add(second[i]);
                        }
                        //then retrun the final list
                        return finalList;
                    }
                }
                //if position one has past the end of list two... 
                if (secondPosition > second.Count - 1)
                {
                    //collect all the remaining elements in list one and
                    //add them to the final list
                    for (int i = firstPosition; i < first.Count; i++)
                    {
                        finalList.Add(first[i]);
                    }
                    //then return the final list
                    return finalList;
                }
                //if the docID at position one and two are equal...
                if (first[firstPosition].DocumentId == second[secondPosition].DocumentId)
                {
                    //add the posting to the final list
                    finalList.Add(first[firstPosition]);
                    //increment both positions up by one
                    firstPosition++;
                    secondPosition++;
                }
                //if the docID at position one is less than the docID at position two
                else if (first[firstPosition].DocumentId < second[secondPosition].DocumentId)
                {
                    //add the posting at position one
                    finalList.Add(first[firstPosition]);
                    //then increment the position up by one
                    firstPosition++;
                }
                //if the docID at position two is less than the docID at position one
                else if (first[firstPosition].DocumentId > second[secondPosition].DocumentId)
                {
                    //add the posting at position two
                    finalList.Add(second[secondPosition]);
                    //then increment position two up by one
                    secondPosition++;
                }
            }
        }


		// PositionalMerge ----------------------------------------------------------------------
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
		private static IList<Posting> PositionalMerge(List<IList<Posting>> list, IList<Posting> last, int gap = 1) {
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
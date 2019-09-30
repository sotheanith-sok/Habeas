using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Search.Index;

namespace Cecs429.Search.Query
{
    public class OrQuery : IQueryComponent
    {
        private List<IQueryComponent> mComponents = new List<IQueryComponent>();

        public IReadOnlyList<IQueryComponent> Components => mComponents;

        public OrQuery(IEnumerable<IQueryComponent> components)
        {
            mComponents.AddRange(components);
        }

        public IList<Posting> GetPostings(IIndex index)
        {
            //list of posting lists from all query components to be OR merged
            List<IList<Posting>> listOfPostingsLists = new List<IList<Posting>>();
            //for each components
            foreach (IQueryComponent qc in mComponents)
            {
                //get a posting list and add it to the collection
                listOfPostingsLists.Add(qc.GetPostings(index));
            }
            return OrMerge(listOfPostingsLists);
        }

        /// <summary>
        /// OR-Merge all posting lists from multiple components into one posting list
        /// </summary>
        /// <param name="list">a list of posting lists to be OR merged</param>
        /// <returns>an OR-merged posting list</returns>
        public static IList<Posting> OrMerge(List<IList<Posting>> list) {
            //exceptions
            if (list.Count == 0) { return new List<Posting>(); }
            if (list.Count == 1) { return list[0]; }

            //Prepare the posting lists to send to the recursive method
            //pop a list off of the end of list
            IList<Posting> first = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //Return the result of recursive merge
            return OrMerge(first, list);
        }

        /// <summary>
        /// OR-Merge a posting list with a list of posting lists with recursion
        /// </summary>
        /// <param name="pList">a posting list to be merged</param>
        /// <param name="listOfPList">a list of posting lists to be merged</param>
        /// <returns>an OR-merged posting list</returns>
        private static IList<Posting> OrMerge(IList<Posting> pList, List<IList<Posting>> listOfPList)
        {
            //base case
            //if listOfPList is empty, all in the list have been merged
            if (listOfPList.Count == 0) { return pList; }
            
            //recursive case
            //TODO: Merge larger posting lists first?
            //pop a list off of the end of listOfPList
            IList<Posting> next = listOfPList[listOfPList.Count - 1];
            listOfPList.RemoveAt(listOfPList.Count - 1);
            
            //return the result of recursion
            return OrMerge( OrMerge(pList, next), listOfPList );
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


        public override string ToString()
        {
            return string.Join(" + ", mComponents.Select(c => c.ToString()));
        }
    }
}

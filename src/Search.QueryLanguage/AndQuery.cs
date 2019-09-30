using System.Collections.Generic;
using System.Linq;
using Search.Index;

namespace Cecs429.Search.Query
{
    public class AndQuery : IQueryComponent
    {
        private List<IQueryComponent> mComponents = new List<IQueryComponent>();

        public IReadOnlyList<IQueryComponent> Components => mComponents;

        public AndQuery(IEnumerable<IQueryComponent> components)
        {
            mComponents.AddRange(components);
        }

        public IList<Posting> GetPostings(IIndex index)
        {
            //candidates will hold the lists of postings for each term or token
            List<IList<Posting>> candidates = new List<IList<Posting>>();
            //for each part of the query...
            foreach (IQueryComponent qc in mComponents)
            {
                //get the postings for that part and add it to candidates
                candidates.Add(qc.GetPostings(index));
            }
            return AndMerge(candidates);
        }

        /// <summary>
        /// AND-Merge all posting lists from multiple components into one posting list
        /// </summary>
        /// <param name="list">a list of posting lists to be OR merged</param>
        /// <returns>an AND-merged posting list</returns>
        public static IList<Posting> AndMerge(List<IList<Posting>> list) {
            //exceptions
            if (list.Count == 0) { return new List<Posting>(); }
            if (list.Count == 1) { return list[0]; }

            //Prepare the posting lists to send to the recursive method
            //pop a list off of the end of list
            IList<Posting> first = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //Return the result of recursive merge
            return AndMerge(first, list);
        }

        /// <summary>
        /// AND-Merge a posting list with a list of posting lists with recursion
        /// </summary>
        /// <param name="pList">a posting list to be merged</param>
        /// <param name="listOfPList">a list of posting lists to be merged</param>
        /// <returns>an AND-merged posting list</returns>
        private static IList<Posting> AndMerge(IList<Posting> pList, List<IList<Posting>> listOfPList)
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
            return AndMerge( AndMerge(pList, next), listOfPList );

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


        public override string ToString()
        {
            return string.Join(" ", mComponents.Select(c => c.ToString()));
        }
    }
}

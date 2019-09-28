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
        /// OR Merge all posting lists from multiple components into one posting list
        /// </summary>
        /// <param name="list">a list of posting lists to be OR merged</param>
        /// <returns></returns>
        public IList<Posting> OrMerge(List<IList<Posting>> list) {
            //exceptions
            if (list.Count == 0) { return new List<Posting>(); }
            if (list.Count == 1) { return list[0]; }

            //Prepare the posting lists to send to the recursive method
            //pop a list off of the end of list
            IList<Posting> first = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            //Return the result of recursive merge
            return MergeLists(first, list);
        }

        //MergeLists is a recursive function
        public IList<Posting> MergeLists(IList<Posting> mergeList, List<IList<Posting>> list)
        {
            //if the list of list is empty, all the lists have been merged together
            //return the mergedList
            if (list.Count == 0) { return mergeList; }
            //if list is not empty...
            else
            {
                //pop a list off of the end of list
                IList<Posting> NextList = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                //or merge the mergeList with the list which was just popped off of list
                mergeList = OrMerge(mergeList, NextList);
                //recursively call MergeLists
                //send in the most recently merged list with the rest of the remaining list
                IList<Posting> FinalListing = MergeLists(mergeList, list);
                //return the resulting listings
                return FinalListing;
            }

        }

        /// <summary>
        /// OR Merge two posting lists into one posting list
        /// </summary>
        /// <param name="first">first posting list to be merged</param>
        /// <param name="second">second posting list to be merged</param>
        /// <returns></returns>
        public IList<Posting> OrMerge(IList<Posting> first, IList<Posting> second)
        {
            //creates a list to hold all the valid postings
            IList<Posting> finalList = new List<Posting>();
            //creates positions for comparing the values of the first and second list of postings
            int firstPosition = 0;
            int secondPosition = 0;
            //if for some reason, both lists are empty, return an empty list
            if (first.Count <= firstPosition && second.Count <= secondPosition) { return finalList; }
            //if the first list is empty, return the second one
            if (first.Count <= firstPosition) { return second; }
            //if the second list is empty, return the first list
            if (second.Count <= secondPosition) { return first; }
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

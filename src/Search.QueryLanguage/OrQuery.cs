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
            //candidates will hold the lists of postings for each term or token
            List<IList<Posting>> candidates = new List<IList<Posting>>();
            //for each part of the query...
            foreach (IQueryComponent qc in mComponents)
            {
                //get the postings for that part and add it to the collection
                candidates.Add(qc.GetPostings(index));
            }
            //if, per some weird error, there is only one part of the query
            //and the program was sent to AndQuery by mistake...
            if (candidates.Count == 1)
            {
                //return the only list of postings there is
                return candidates[candidates.Count - 1];
            }
            //pop a list off of the end of candidates
            IList<Posting> firstList = candidates[candidates.Count - 1];
            candidates.RemoveAt(candidates.Count - 1);
            //send the popped list and the rest of the candidates to the function MergeLists
            IList<Posting> finalPostingList = MergeLists(firstList, candidates);
            //MergeLists will create the final list to be returned
            return finalPostingList;
        }

        public override string ToString()
        {
            return string.Join(" ", mComponents.Select(c => c.ToString()));
        }

        //MergeLists is a recursive function
        public IList<Posting> MergeLists(IList<Posting> mergeList, List<IList<Posting>> candidates)
        {
            //if the list of candidates is empty, all the lists have been merged together
            //return the mergedList
            if (candidates.Count == 0) { return mergeList; }
            //if candidates is not empty...
            else
            {
                //pop a list off of the end of candidates
                IList<Posting> NextList = candidates[candidates.Count - 1];
                candidates.RemoveAt(candidates.Count - 1);
                //or merge the mergeList with the list which was just popped off of candidates
                mergeList = OrMerge(mergeList, NextList);
                //recursively call MergeLists
                //send in the most recently merged list with the rest of the remaining candidates
                IList<Posting> FinalListing = MergeLists(mergeList, candidates);
                //return the resulting listings
                return FinalListing;
            }

        }

        public IList<Posting> OrMerge(IList<Posting> firstList, IList<Posting> secondList)
        {
            //creates a list to hold all the valid postings
            IList<Posting> finalList = new List<Posting>();
            //creates positions for comparing the values of the first and second list of postings
            int firstListPosition = 0;
            int secondListPosition = 0;
            //if for some reason, both lists are empty, return an empty list
            if (firstList.Count <= firstListPosition && secondList.Count <= secondListPosition) { return finalList; }
            //if the first list is empty, return the second one
            if (firstList.Count <= firstListPosition) { return secondList; }
            //if the second list is empty, return the first list
            if (secondList.Count <= secondListPosition) { return firstList; }
            //if both lists have postings in them...
            while (true)
            {
                //if position one has past the end of list one... 
                if (firstListPosition > firstList.Count - 1)
                {
                    //check to see if position two has past the end of list two
                    //if so, return whatever is in the final list
                    if (secondListPosition > secondList.Count - 1) { return finalList; }
                    //otherwise, if there are some elements still in the second list
                    else
                    {
                        //iterate through the second list and add all the remaining elements to the final list
                        for (int i = secondListPosition; i < secondList.Count; i++)
                        {
                            finalList.Add(secondList[i]);
                        }
                        //then retrun the final list
                        return finalList;
                    }
                }
                //if position one has past the end of list two... 
                if (secondListPosition > secondList.Count - 1)
                {
                    //collect all the remaining elements in list one and
                    //add them to the final list
                    for (int i = firstListPosition; i < firstList.Count; i++)
                    {
                        finalList.Add(firstList[i]);
                    }
                    //then return the final list
                    return finalList;
                }
                //if the docID at position one and two are equal...
                if (firstList[firstListPosition].DocumentId == secondList[secondListPosition].DocumentId)
                {
                    //add the posting to the final list
                    finalList.Add(firstList[firstListPosition]);
                    //increment both positions up by one
                    firstListPosition++;
                    secondListPosition++;
                }
                //if the docID at position one is less than the docID at position two
                else if (firstList[firstListPosition].DocumentId < secondList[secondListPosition].DocumentId)
                {
                    //add the posting at position one
                    finalList.Add(firstList[firstListPosition]);
                    //then increment the position up by one
                    firstListPosition++;
                }
                //if the docID at position two is less than the docID at position one
                else if (firstList[firstListPosition].DocumentId > secondList[secondListPosition].DocumentId)
                {
                    //add the posting at position two
                    finalList.Add(secondList[secondListPosition]);
                    //then increment position two up by one
                    secondListPosition++;
                }
            }
        }
    }
}

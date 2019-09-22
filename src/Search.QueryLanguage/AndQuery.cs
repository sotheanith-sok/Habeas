using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            //if, per some weird error, there is only one part of the query
            //and the program was sent to AndQuery by mistake...
            if (candidates.Count == 1)
            {
                //return the only list of postings there is
                return candidates[candidates.Count - 1];
            }
            //pop one list off of the back of candidates
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
                //and merge the mergeList with the list which was just popped off of candidates
                mergeList = AndMerge(mergeList, NextList);
                //recursively call MergeLists
                //send in the most recently merged list with the rest of the remaining candidates
                IList<Posting> FinalListing = MergeLists(mergeList, candidates);
                //return the resulting listings
                return FinalListing;
            }

        }

        public IList<Posting> AndMerge(IList<Posting> firstList, IList<Posting> secondList)
        {
            //creates a list to hold all the valid postings
            IList<Posting> finalList = new List<Posting>();
            //creates positions for comparing the values of the first and second list of postings
            int firstListPosition = 0;
            int secondListPosition = 0;
            //if for some reason, one of the lists is empty, simply return an empty list
            if (firstList.Count <= firstListPosition || secondList.Count <= secondListPosition) { return finalList; }
            //if both lists have postings in them...
            while (true)
            {
                //if the position has past the end of either list, the AND merge is complete  
                if (firstListPosition > firstList.Count - 1 || secondListPosition > secondList.Count - 1) { return finalList; }
                //if the documentID of both postings are the same
                if (firstList[firstListPosition].DocumentId == secondList[secondListPosition].DocumentId)
                {
                    //add that posting to the final list
                    finalList.Add(firstList[firstListPosition]);
                    //move both positions up by one 
                    firstListPosition++;
                    secondListPosition++;
                }
                //if the documentID of the firstList is less than the DocumentID of the second
                else if (firstList[firstListPosition].DocumentId < secondList[secondListPosition].DocumentId)
                {
                    //move position of the firstList up by one
                    firstListPosition++;
                }
                //if the documentID of the firstList is greater than the DocumentID of the second
                else if (firstList[firstListPosition].DocumentId > secondList[secondListPosition].DocumentId)
                {
                    //move position of the secondList up by one
                    secondListPosition++;
                }
            }
        }
    }
}

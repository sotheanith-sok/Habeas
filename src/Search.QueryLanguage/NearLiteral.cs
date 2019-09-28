using System.Collections.Generic;
using Cecs429.Search.Query;
using Search.Index;

namespace Search.Query
{
    /// <summary>
    /// NearLiteral is a query component type
    /// representing the three parts of a NEAR query "[token1 NEAR/k token2]"
    /// </summary>
    public class NearLiteral : IQueryComponent
    {
        private string firstTerm;
        private int k;
        private string secondTerm;

        /// <summary>
        /// Constructs NearLiteral with two terms and the k value between the two
        /// </summary>
        /// <param name="first">the first token</param>
        /// <param name="k">a value of how far the second is away from the first at most</param>
        /// <param name="second">the second token</param>
        public NearLiteral(string first, int k, string second) {
            //NOTE: should this be token?term? already processed? or not?
            this.firstTerm = first;
            this.k = k;
            this.secondTerm = second;
        }

        public IList<Posting> GetPostings(IIndex index)
        {
            //Get postings for the two term
            IList<Posting> firstPostings = index.GetPostings(firstTerm);
            IList<Posting> secondPostings = index.GetPostings(secondTerm);
            
            //PositionalMerge with gap(distance) 1 to k
            List<IList<Posting>> candidates = new List<IList<Posting>>();
            for(int i=1; i<=k; i++) {
                candidates.Add(Merge.PositionalMerge(firstPostings, secondPostings, i));
            }
            //TODO: OrMerge all of them
            // return Merge.OrMerge(candidates);

            throw new System.NotImplementedException();
        }

        public override string ToString() {
			return "[" + firstTerm + " NEAR/"+k+" " + secondTerm + "]";
		}
    }
}
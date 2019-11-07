using System.Collections.Generic;
using Search.Index;
using Search.Text;

namespace Search.Query
{
    /// <summary>
    /// NearLiteral is a query component type
    /// representing the three parts of a NEAR query "[token1 NEAR/k token2]"
    /// </summary>
    public class NearLiteral : IQueryComponent
    {
        private string firstTerm;   //not processed word
        private int k;
        private string secondTerm;  //not processed word

        /// <summary>
        /// Constructs NearLiteral with two terms and the k value between the two
        /// </summary>
        /// <param name="first">the first token</param>
        /// <param name="k">a value of how far the second is away from the first at most</param>
        /// <param name="second">the second token</param>
        public NearLiteral(string first, int k, string second)
        {
            this.firstTerm = first;
            this.k = k;
            this.secondTerm = second;
        }

        public IList<Posting> GetPostings(IIndex index, ITokenProcessor processor)
        {
            //Get postings for the two term
            List<string> termsFromFirst = processor.ProcessToken(firstTerm);
            List<string> termsFromSecond = processor.ProcessToken(secondTerm);
            IList<Posting> firstPostings = index.GetPositionalPostings(termsFromFirst);
            IList<Posting> secondPostings = index.GetPositionalPostings(termsFromSecond);

            //PositionalMerge to any postings found with gap(distance) 1 to k (up to k)
            List<IList<Posting>> list = new List<IList<Posting>>();
            for (int i = 1; i <= k; i++)
            {
                list.Add(Merge.PositionalMerge(firstPostings, secondPostings, i));
            }

            //OrMerge all of them
            return Merge.OrMerge(list);
        }

        public override string ToString()
        {
            return "[" + firstTerm + " NEAR/" + k + " " + secondTerm + "]";
        }
    }
}
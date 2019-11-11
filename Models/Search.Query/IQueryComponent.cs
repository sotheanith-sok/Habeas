using System.Collections.Generic;
using Search.Index;
using Search.Text;

namespace Search.Query
{
    public interface IQueryComponent
    {
        /// <summary>
        /// Get Postings
        /// </summary>
        /// <param name="index">Index</param>
        /// <param name="processor">Tokene processor</param>
        /// <returns></returns>
        IList<Posting> GetPostings(IIndex index, ITokenProcessor processor);
    }
}

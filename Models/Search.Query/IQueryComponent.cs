using System.Collections.Generic;
using Search.Index;
using Search.Text;

namespace Search.Query
{
    public interface IQueryComponent
    {
        IList<Posting> GetPostings(IIndex index, ITokenProcessor processor);
    }
}

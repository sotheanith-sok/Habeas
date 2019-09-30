using System.Collections.Generic;
using System.Linq;
using Search.Index;
using Search.Query;

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
            //list of posting lists from all query components to be AND-merged
            List<IList<Posting>> listOfPostingsLists = new List<IList<Posting>>();
            //for each components
            foreach (IQueryComponent qc in mComponents)
            {
                //get a posting list and add it to the collection
                listOfPostingsLists.Add(qc.GetPostings(index));
            }
            return Merge.AndMerge(listOfPostingsLists);
        }

        public override string ToString()
        {
            return string.Join(" ", mComponents.Select(c => c.ToString()));
        }
    }
}

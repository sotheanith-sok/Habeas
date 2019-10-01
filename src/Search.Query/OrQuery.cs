using System.Collections.Generic;
using System.Linq;
using Search.Index;

namespace Search.Query
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
            return Merge.OrMerge(listOfPostingsLists);
        }

        public override string ToString()
        {
            return string.Join(" + ", mComponents.Select(c => c.ToString()));
        }
    }
}

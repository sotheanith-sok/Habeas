using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Search.Index;

namespace Cecs429.Search.Query {
	public class OrQuery : IQueryComponent {
		private List<IQueryComponent> mComponents = new List<IQueryComponent>();

		public IReadOnlyList<IQueryComponent> Components => mComponents;

		public OrQuery(IEnumerable<IQueryComponent> components) {
			mComponents.AddRange(components);
		}

		public IList<Posting> GetPostings(IIndex index) {
			IList<Posting> finalPostingList = new List<Posting>();
			List<IList<Posting>> candidates = new List<IList<Posting>>();
			foreach(IQueryComponent qc in mComponents){
				candidates.Add(qc.GetPostings(index));
			}
			candidates.OrderByDescending(x => x.Count);
			

			//OR merge them together.
			return finalPostingList;
		}


		public override string ToString() {
			return string.Join(" + ", mComponents.Select(c => c.ToString()));
		}
	}
}

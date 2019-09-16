using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cecs429.Search.Index;

namespace Cecs429.Search.Query {
	public class OrQuery : IQueryComponent {
		private List<IQueryComponent> mComponents = new List<IQueryComponent>();

		public IReadOnlyList<IQueryComponent> Components => mComponents;

		public OrQuery(IEnumerable<IQueryComponent> components) {
			mComponents.AddRange(components);
		}

		public IList<Posting> GetPostings(IIndex index) {
			throw new NotImplementedException();
			// TODO: program this method. Retrieve the postings for the individual query components in
			// mComponents, and OR merge them together.
		}


		public override string ToString() {
			return string.Join(" + ", mComponents.Select(c => c.ToString()));
		}
	}
}

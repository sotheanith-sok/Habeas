using System;
using System.Collections.Generic;
using Search.Index;

namespace Cecs429.Search.Query {
	public interface IQueryComponent {
		IList<Posting> GetPostings(IIndex index);
	}
}

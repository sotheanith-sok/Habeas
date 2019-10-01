using System;
using System.Collections.Generic;
using Search.Index;

namespace Search.Query {
	public interface IQueryComponent {
		IList<Posting> GetPostings(IIndex index);
	}
}

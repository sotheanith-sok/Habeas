using System;
using System.Collections.Generic;
using Cecs429.Search.Index;

namespace Cecs429.Search.Query {
	public interface IQueryComponent {
		IList<Posting> GetPostings(IIndex index);
	}
}

namespace Cecs429.Search.Index {
	/// <summary>
	/// A Posting represents a document that contains a term from a query.
	/// </summary>
	public class Posting {
		/// <summary>
		/// The 0-based unique ID of the document.
		/// </summary>
		public int DocumentId { get;  }

		public Posting(int documentID) {
			DocumentId = documentID;
		}
	}
}
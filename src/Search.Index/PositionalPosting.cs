using System.Collections.Generic;

namespace Search.Index {
	/// <summary>
	/// A PositionalPosting represents a document and positions of a term from a query.
	/// </summary>
	public class PositionalPosting {
		/// <summary>
		/// The 0-based unique ID of the document.
		/// </summary>
		public int DocumentId { get;  }

        /// <summary>
        /// The list of positions of a term(posting) within the document.
        /// </summary>
        public List<int> Positions { get; }

        /// <summary>
        /// Constructs a Positional Posting with a docID and positions of a term
        /// </summary>
        public PositionalPosting(int docID, List<int> positions){
            DocumentId = docID;
            Positions = positions;
        }
	}
}
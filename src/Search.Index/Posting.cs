using System.Collections.Generic;

namespace Search.Index {
	/// <summary>
	/// A Posting represents a documentID and positions of a term from a query.
    /// e.g. (docID, [pos1, pos2, pos3])
	/// </summary>
	public class Posting {
		/// <summary>
		/// The 0-based unique ID of the document.
		/// </summary>
		public int DocumentId { get;  }

        /// <summary>
        /// The list of positions of a term(posting) within the document.
        /// </summary>
        public List<int> Positions { get; }

        /// <summary>
        /// Constructs a Positional Posting with a docID and positions of a term.
        /// </summary>
        /// <param name="docID">ID of a document where a term is in</param>
        /// <param name="positions">positions of a term in a document</param>
        public Posting(int docID, List<int> positions){
            DocumentId = docID;
            Positions = positions;
        }

        /// <summary>
        /// Returns a string of this posting in (docId, [pos1, pos2, ... ]) form
        /// </summary>
        override public string ToString(){
            string str = "(" + DocumentId + ", [";
            Positions.ForEach(pos => str = str + pos + ",");
            str += "])";
            return str;
        }
	}
}
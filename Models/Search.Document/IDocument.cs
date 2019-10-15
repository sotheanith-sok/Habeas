using System.IO;

namespace Search.Document
{
    /// <summary>
    /// Represents a document in an index.
    /// </summary>
    public interface IDocument
    {
        /// <summary>
        /// The ID used by the index to represent the document.
        /// </summary>
        int DocumentId { get; }

        /// <summary>
        /// The title of the document, for displaying to the user.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// The author of the document
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets a stream over the content of the document.
        /// </summary>
        TextReader GetContent();

    }
}

namespace Search.Document
{
    /// <summary>
    /// Represents a document saved as a file on the local file system.
    /// </summary>
    public interface IFileDocument : IDocument
    {
        /// <summary>
        /// The absolute path to the file for the document.
        /// </summary>
        new string FilePath { get; }

        /// <summary>
        /// Name of  the file including extension
        /// </summary>
        string FileName { get; }
    }
}
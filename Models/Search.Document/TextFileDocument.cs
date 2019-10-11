using System;
using System.IO;
using System.IO.MemoryMappedFiles;

namespace Search.Document
{
    /// <summary>
    /// Represents a document that is saved as a simple text file in the local file system.
    /// </summary>
    public class TextFileDocument : IFileDocument, IDisposable
    {
        public int DocumentId { get; }
        /// <summary>
        /// The absolute path to the document's file.
        /// </summary>
        public string FilePath { get; }
        public string FileName { get; }
        public string Title { get; }
        public string Author { get; }

        private MemoryMappedFile file;

        /// <summary>
        /// Constructs a TextFileDocument with the given document ID representing the file at the given 
        /// absolute file path.
        /// </summary>
        public TextFileDocument(int id, string absoluteFilePath)
        {
            DocumentId = id;
            FilePath = absoluteFilePath;
            FileName = Path.GetFileName(absoluteFilePath);
            Title = FileName;   //since text file doesn't have title field
        }

        /// <summary>
        /// Get content of a text file
        /// </summary>
        public TextReader GetContent()
        {
            this.file = MemoryMappedFile.CreateFromFile(FilePath);
            return new StreamReader(this.file.CreateViewStream());
        }

        /// <summary>
        /// A factory method for constructing basic text documents that consist solely of content.
        /// </summary>
        public static TextFileDocument CreateTextFileDocument(string absoluteFilePath, int documentId)
        {
            return new TextFileDocument(documentId, absoluteFilePath);
        }

        public void Dispose()
        {
            file?.Dispose();
        }
    }
}

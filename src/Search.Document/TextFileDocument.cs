using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;

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

        public string Title { get; }

        private MemoryMappedFile file;

        /// <summary>
        /// Constructs a TextFileDocument with the given document ID representing the file at the given 
        /// absolute file path.
        /// </summary>
        public TextFileDocument(int id, string absoluteFilePath)
        {
            DocumentId = id;
            FilePath = absoluteFilePath;
            Title = Path.GetFileName(absoluteFilePath);
        }

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

        public void Dispose(){
            file?.Dispose();
        }
    }
}

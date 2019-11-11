
using System.IO;
using System.IO.MemoryMappedFiles;
using System;
using Newtonsoft.Json;
namespace Search.Document
{
    /// <summary>
    /// Document class specific for JSON files
    /// </summary>
    public class Document
    {
        public string title { get; set; }
        public string body { get; set; }
        public string url { get; set; }
        public string author { get; set; }
    }

    public class JsonFileDocument : IFileDocument
    {

        public int DocumentId { get; }
        /// <summary>
        /// The absolute path to the document's file.
        /// </summary>
        public string FilePath { get; }
        public string FileName { get; }
        public string Title { get; set; }
        public string Author { get; set; }

        public JsonFileDocument(int documentId, string absoluteFilePath)
        {
            DocumentId = documentId;
            FilePath = absoluteFilePath;
            FileName = Path.GetFileName(absoluteFilePath);

            StreamReader fileStreamReader = new StreamReader(FileManager.Instance.GetFile(this.FilePath));
            Document jobject = JsonConvert.DeserializeObject<Document>(fileStreamReader.ReadToEnd());
            Title = (jobject.title != null) ? jobject.title : "";
            Author = jobject.author;
            fileStreamReader.Dispose();
        }

        /// <summary>
        /// Get content of a json file
        /// </summary>
        /// <returns></returns>
        public TextReader GetContent()
        {
            StreamReader fileStreamReader = new StreamReader(FileManager.Instance.GetFile(this.FilePath));
            Document jobject = JsonConvert.DeserializeObject<Document>(fileStreamReader.ReadToEnd());
            var content = (jobject.body != null) ? jobject.body : "";
            fileStreamReader.Dispose();
            return new StringReader(content);
        }

        /// <summary>
        /// Create JsonFileDocument
        /// </summary>
        /// <param name="absoluteFilePath">path to file</param>
        /// <param name="documentId">document ID</param>
        /// <returns></returns>
        public static JsonFileDocument CreateJsonFileDocument(string absoluteFilePath, int documentId)
        {
            return new JsonFileDocument(documentId, absoluteFilePath);
        }

    }
}
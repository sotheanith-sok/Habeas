
using System.IO;
using System.IO.MemoryMappedFiles;
using System;
using Newtonsoft.Json;
namespace Search.Document
{



    public class Document
    {
        public string title { get; set; }
        public string body { get; set; }
        public string url { get; set; }

        public string author{get;set;}
    }
    
    public class JsonFileDocument : IFileDocument
    {

        public int DocumentId { get; }
        /// <summary>
        /// The absolute path to the document's file.
        /// </summary>
        public string FilePath { get; }

        public string Title { get;}
        
        public string Author {get; set;}
        public string articleTitle{get; set;}

        public JsonFileDocument(int documentId, string absoluteFilePath)
        {
            
            DocumentId = documentId;
            FilePath = absoluteFilePath;
            Title = Path.GetFileName(absoluteFilePath);
            
        }

        
        public TextReader GetContent()
        {
            StreamReader file = new StreamReader(MemoryMappedFile.CreateFromFile(FilePath).CreateViewStream());
            Document jobject = JsonConvert.DeserializeObject<Document>(file.ReadToEnd());
            articleTitle = jobject.title;
            Author = jobject.author;
            var content = jobject.title + jobject.body + jobject.url+Author;
            file.Dispose();
            return new StringReader(content);
        }


        public static JsonFileDocument CreateJsonFileDocument(string absoluteFilePath, int documentId)
        {
            return new JsonFileDocument(documentId, absoluteFilePath);
        }


    }
}
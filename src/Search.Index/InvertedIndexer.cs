using Search.Document;
using Search.Index;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Search.InvertedIndexer
{
    public class InvertedIndexer
    {
        public static void Main(string[] args)
        {
            // Using Moby-Dick chapters as a corpus for now.
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./corpus", ".txt");

            IIndex index = IndexCorpus(corpus);
            // We only support single-term queries for now.

            string query;
            int count;

            while(true){
                Console.Write("Search: ");
                query = Console.ReadLine();

                if(query == ":q"){
                    break;
                }
                else if(query.StartsWith(":stem ")){
                    Console.Write(query.Substring(6));
                }
                else if(query.StartsWith(":index ")){
                    //IDocumentCorpus corpus = DirectoryCorpus.LoadJsonDirectory(query.Substring7, ".json");
                    index = IndexCorpus(corpus); 
                }
                else if(query.StartsWith(":vocab")){
                    IReadOnlyList<string> vocabList = index.GetVocabulary();
                    
                    foreach (string term in vocabList.Take(1000)) {
                        Console.WriteLine(term);
                    }

                    //Fancy
                    //Console.WriteLine(string.Join('\n', vocabList.Take(1000)));

                    Console.WriteLine(vocabList.Count);
                }
                else{

                count = 0;
                foreach (Posting p in index.GetPostings(query)) {
                    Console.WriteLine($"Document  {corpus.GetDocument(p.DocumentId).Title}");
                    count += 1;
                }
                Console.WriteLine($"'{query}' found in {count} files\n");
                }
            }
            
        }

        /// <summary>
        /// Index a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        private static IIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BasicTokenProcessor();

            // Constuct a inverted-index once 
            InvertedIndex index = new InvertedIndex();

            Console.WriteLine("Indexing the corpus...");
            // Index the document
            foreach (IDocument doc in corpus.GetDocuments())
            {
                //Tokenize the documents
                ITokenStream stream = new EnglishTokenStream(doc.GetContent());
                IEnumerable<string> tokens = stream.GetTokens();

                foreach (string token in tokens) {
                    //Process token to term
                    string term = processor.ProcessToken(token);
                    //Add term to the index
                    if(term.Length > 0) {
                        index.AddTerm(term, doc.DocumentId);
                    }
                }
                stream.Dispose();
            }

            return index;
        }



    }
}

using Search.Document;
using Search.Index;
using Search.Text;
using System;
using System.Collections.Generic;

namespace Search.PositionalInvertedIndexer
{
    public class PositionalInvertedIndexer
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

                count = 0;
                //TODO: change GetPostings() to getPositionalPostings()
                foreach (Posting p in index.GetPostings(query)) {
                    Console.WriteLine($"Document  {corpus.GetDocument(p.DocumentId).Title}");
                    count += 1;
                }
                Console.WriteLine($"'{query}' found in {count} files\n");
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
            PositionalInvertedIndex index = new PositionalInvertedIndex();

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
                        //TODO: pass a proper position parameter
                        index.AddTerm(term, doc.DocumentId, 0);
                    }
                }
                stream.Dispose();
            }

            return index;
        }



    }
}
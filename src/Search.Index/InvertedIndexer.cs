using Search.Document;
using Search.Index;
using Search.Text;
using System;
using System.Collections.Generic;

namespace Search.InvertedIndexer
{
    public class InvertedIndexer
    {
        // public static void Main(string[] args)
        // {
        //     // Using Moby-Dick chapters as a corpus for now.
        //     IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./corpus", ".txt");

        //     IIndex index = IndexCorpus(corpus);
        //     // We only support single-term queries for now.

        //     string query;

        //     while(true){
        //         Console.Write("Search: ");
        //         query = Console.ReadLine();

        //         if(query == ":q"){
        //             break;
        //         }

        //         IList<Posting> postings = index.GetPostings(query);
        //         foreach (Posting p in postings) {
        //             Console.WriteLine($"Document  {corpus.GetDocument(p.DocumentId).Title}");
        //         }
        //         Console.WriteLine($"'{query}' found in {postings.Count} files\n");
        //     }
            
        // }

        /// <summary>
        /// Index a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        private static IIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BasicTokenProcessor();

            // Constuct a inverted-index once 
            InvertedIndex index = new InvertedIndex();

            Console.WriteLine("Indexing the corpus... with Inverted Index");
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
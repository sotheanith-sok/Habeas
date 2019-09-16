
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

            PositionalInvertedIndex index = IndexCorpus(corpus);
            // We only support single-term queries for now.

            string query;
            IList<PositionalPosting> postings;

            while(true) {
                Console.Write("Search: ");
                query = Console.ReadLine();

                if(query == ":q") {
                    break;
                }

                postings = index.GetPositionalPostings(query);
                //TODO: Change GetPositionalPostings() to updated GetPostings()
                foreach (PositionalPosting p in postings)
                {
                    Console.WriteLine($"Document  {corpus.GetDocument(p.DocumentId).Title}");
                }
                Console.WriteLine($"'{query}' found in {postings.Count} files\n");
            }
        }

        /// <summary>
        /// Index a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        private static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BasicTokenProcessor();

            // Constuct a positional-inverted-index once 
            PositionalInvertedIndex index = new PositionalInvertedIndex();

            Console.WriteLine("Indexing the corpus... with Positional Inverted Index");
            // Index the document
            foreach (IDocument doc in corpus.GetDocuments())
            {
                //Tokenize the documents
                ITokenStream stream = new EnglishTokenStream(doc.GetContent());
                IEnumerable<string> tokens = stream.GetTokens();

                int position = 0;
                foreach (string token in tokens) {
                    //Process token to term
                    string term = processor.ProcessToken(token);
                    //Add term to the index
                    if(term.Length > 0) {
                        index.AddTerm(term, doc.DocumentId, position);
                    }
                    //Increase the position num
                    position += 1;
                }

                stream.Dispose();
            }

            return index;
        }



    }
}
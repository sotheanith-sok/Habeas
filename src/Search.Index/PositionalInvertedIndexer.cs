
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
            string _directory = "./corpus";
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(_directory, ".txt");
            PositionalInvertedIndex index = IndexCorpus(corpus);

            string query;
            IList<Posting> postings;

            while(true) {
                Console.Write("Search: ");
                query = Console.ReadLine();

                if(query == ":q") {
                    break;
                } else if (query == ":vocab") {
                    PrintVocab();
                }


                //TODO: Change GetPostings() to updated GetPostings()
                postings = index.GetPostings(query);
                foreach (Posting p in postings)
                {
                    Console.Write($"Document {corpus.GetDocument(p.DocumentId).Title}");
                    Console.Write($"\t{p.ToString()}");
                    Console.WriteLine();
                }
                
                Console.WriteLine($"'{query}' found in {postings.Count} files\n");
                
            }
        }

        /// <summary>
        /// Index a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        public static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
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

        public static void PrintVocab(){

        }

    }
}
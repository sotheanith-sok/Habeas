
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
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(_directory);
            PositionalInvertedIndex index = IndexCorpus(corpus);

            string query;
            IList<Posting> postings;

            while(true) {
                Console.Write("\nSearch: ");
                query = Console.ReadLine();

                if (query == ":q") {
                    break;
                }
                else if (query == ":vocab") {
                    PrintVocab(index.GetVocabulary(), 100);
                }
                else {
                    postings = index.GetPostings(query);
                    foreach (Posting p in postings)
                    {
                        Console.Write($"Document {corpus.GetDocument(p.DocumentId).Title}");
                        Console.Write($"\t{p.ToString()}");
                        Console.WriteLine();
                    }
                    Console.WriteLine($"'{query}' found in {postings.Count} files");
                }
            }
        }

        /// <summary>
        /// Constructs an index from a corpus of documents
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

        /// <summary>
        /// Prints the first 1000 terms in the sorted vocabulary and the count
        /// </summary>
        /// <param name="vocabulary">a sorted vocabulary to print</param>
        /// <param name="count">the number of terms to print</param>
        public static void PrintVocab(IReadOnlyList<string> vocabulary, int count){
            for(int i=0; i < Math.Min(count, vocabulary.Count); i++) {
                Console.WriteLine(vocabulary[i]);
            }
            Console.WriteLine($"Total: {vocabulary.Count} terms");
        }

    }
}
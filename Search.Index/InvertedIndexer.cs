using Search.Document;
using Search.Index;
using Search.Text;
using System;
using System.Collections.Generic;
namespace Search.InvertedIndexer
{
    public class InvertedIndexer
    {
        public static void Main(string[] args)
        {
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./corpus", ".txt");

            IIndex index = IndexCorpus(corpus);
            // We aren't ready to use a full query parser; for now, we'll only support single-term queries.
            string query;
            do
            {
                Console.Write("Enter your search:");
                query = Console.ReadLine();
                if (query == ":q")
                {
                    break;
                }

                foreach (Posting p in index.GetPostings(query))
                {
                    Console.WriteLine($"Document  {corpus.GetDocument(p.DocumentId).Title}");
                }

            } while (query != ":q");
        }

        private static IIndex IndexCorpus(IDocumentCorpus corpus)
        {
            BasicTokenProcessor processor = new BasicTokenProcessor();


            // TODO:
            // Constuct a TermDocumentMatrix once you know the size of the vocabulary. 
            // THEN, do the loop again! But instead of inserting into the HashSet, add terms to the index with addPosting.
            InvertedIndex index = new InvertedIndex();
            foreach (IDocument i in corpus.GetDocuments())
            {
                EnglishTokenStream tokensStream = new EnglishTokenStream(i.GetContent());
                IEnumerable<string> tokens = tokensStream.GetTokens();
                foreach (string s in tokens)
                {
                    index.AddTerm(processor.ProcessToken(s), i.DocumentId);
                }
                tokensStream.Dispose();
            }

            return index;
        }



    }
}
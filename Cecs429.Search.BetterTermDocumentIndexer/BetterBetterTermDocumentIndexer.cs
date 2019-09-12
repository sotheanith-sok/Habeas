using Cecs429.Search.Documents;
using Cecs429.Search.Index;
using Cecs429.Search.Text;
using System;
using System.Collections.Generic;
namespace Cecs429.Search.BetterBetterTermDocumentIndexer
{
    public class BetterBetterTermDocumentIndexer
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
                if (query == "quit")
                {
                    break;
                }

                foreach (Posting p in index.GetPostings(query))
                {
                    Console.WriteLine($"Document  {corpus.GetDocument(p.DocumentId).Title}");
                }

            } while (query != "quit");
        }

        private static IIndex IndexCorpus(IDocumentCorpus corpus)
        {
            HashSet<string> vocabulary = new HashSet<string>();
            BasicTokenProcessor processor = new BasicTokenProcessor();


            // TODO:
            // Constuct a TermDocumentMatrix once you know the size of the vocabulary. 
            // THEN, do the loop again! But instead of inserting into the HashSet, add terms to the index with addPosting.
            TermDocumentInvertedIndex tdii = new TermDocumentInvertedIndex();
            foreach (IDocument i in corpus.GetDocuments())
            {
                EnglishTokenStream tokensStream = new EnglishTokenStream(i.GetContent());
                IEnumerable<string> tokens = tokensStream.GetTokens();
                foreach (string s in tokens)
                {
                    tdii.AddTerm(processor.ProcessToken(s), i.DocumentId);
                }
                tokensStream.Dispose();
            }

            return tdii;
        }



    }
}
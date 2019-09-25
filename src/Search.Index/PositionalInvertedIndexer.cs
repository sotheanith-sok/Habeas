
using Search.Document;
using Search.Index;
using Search.Text;
using System;
using System.Collections.Generic;


namespace Search.PositionalInvertedIndexer
{
    public class PositionalInvertedIndexer
    {

        /// <summary>
        /// Constructs an index from a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        public static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BetterTokenProcessor();

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
                foreach (string token in tokens)
                {
                    //Process token to term
                    List<string> terms = processor.ProcessToken(token, false, false);
                    //Add term to the index
                    foreach (string term in terms)
                    {
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
        public static void PrintVocab(IReadOnlyList<string> vocabulary, int count)
        {
            for (int i = 0; i < Math.Min(count, vocabulary.Count); i++)
            {
                Console.WriteLine(vocabulary[i]);
            }
            Console.WriteLine($"Total: {vocabulary.Count} terms");
        }

    }
}

using Search.Document;
using Search.Index;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Search.Index
{
    public class PositionalInvertedIndexer
    {

        public static KGram kGram = null;
        /// <summary>
        /// Constructs an index from a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        public static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            //Time how long it takes to index the corpus
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();
            
            // Constuct a positional-inverted-index once 
            PositionalInvertedIndex index = new PositionalInvertedIndex();
            Console.WriteLine($"Indexing {corpus.CorpusSize} documents in the corpus...");
            ITokenProcessor normalProcessor = new NormalTokenProcessor();

            Console.WriteLine("Indexing the corpus... with Positional Inverted Index");

            HashSet<string> tokenSet = new HashSet<string>();

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
                    List<string> terms = normalProcessor.ProcessToken(token);
                    //Add term to the index
                    foreach (string term in terms)
                    {
                        if (term.Length > 0)
                        {
                            index.AddTerm(term, doc.DocumentId, position);
                        }
                    }
                    //Increase the position num
                    position += 1;

                    //Keep track of vocabularies for K-gram
                    foreach (string term in normalProcessor.ProcessToken(token)){
                        tokenSet.Add(term);
                    }
                }

                stream.Dispose();
                ((IDisposable) doc).Dispose();
            }
            kGram = new KGram(tokenSet);
            elapsedTime.Stop();
            Console.WriteLine("Elapsed " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
            return index;
        }

    }
}
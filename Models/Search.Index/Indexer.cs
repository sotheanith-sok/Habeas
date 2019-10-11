
using Search.Document;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Search.Index
{
    public class Indexer
    {

        public static KGram kGram = null;
        public static SoundExIndex soundEx = null;

        /// <summary>
        /// Constructs an index from a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        public static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            //Time how long it takes to index the corpus
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();
            soundEx = new SoundExIndex();
            // Constuct a positional-inverted-index once 
            PositionalInvertedIndex index = new PositionalInvertedIndex();
            Console.WriteLine($"Indexing {corpus.CorpusSize} documents in the corpus...");

            ITokenProcessor processor = new StemmingTokenProcesor();

            HashSet<string> unstemmedVocabulary = new HashSet<string>();

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
                    List<string> terms = processor.ProcessToken(token);
                    //Add term to the index
                    foreach (string term in terms)
                    {
                        if (term.Length > 0)
                        {
                            index.AddTerm(term, doc.DocumentId, position);
                            // Console.WriteLine("¾"); //This thing will print out as _. So index is correct.
                        }
                    }
                    //Increase the position num
                    position += 1;

                    //Keep track of vocabularies for K-gram
                    foreach (string term in ((NormalTokenProcessor)processor).ProcessToken(token))
                    {
                        unstemmedVocabulary.Add(term);
                    }
                }
                soundEx.AddDocIdByAuthor(doc.Author, doc.DocumentId);
                stream.Dispose();
                ((IDisposable)doc).Dispose();

            }
            kGram = new KGram(unstemmedVocabulary);

            elapsedTime.Stop();
            Console.WriteLine("Elapsed " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));

            return index;
        }

    }
}
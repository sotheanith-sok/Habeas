
using Search.Document;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Timers;

namespace Search.Index
{
    public class Indexer
    {

        public static string path = "./";


        /// <summary>
        /// Constructs an index from a corpus of documents
        /// </summary>
        /// <param name="corpus">a corpus to be indexed</param>
        public static IIndex IndexCorpus(IDocumentCorpus corpus)
        {
            Console.WriteLine($"[Indexer] Indexing {corpus.CorpusSize} documents in the corpus...");

            // Time how long it takes to index the corpus
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();

            // Set the index type and token processor to use
            SpecialIndex index = new SpecialIndex(Indexer.path);
            ITokenProcessor processor = new StemmingTokenProcesor();

            HashSet<string> unstemmedVocabulary = new HashSet<string>();
            SortedDictionary<string, List<int>> soundEx = new SortedDictionary<string, List<int>>();

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
                    bool termsIsAdded = false;
                    foreach (string term in terms)
                    {
                        if (term.Length > 0)
                        {
                            Console.WriteLine(position + ":" + term);
                            index.AddTerm(term, doc.DocumentId, position);
                            termsIsAdded = true;
                            // Console.WriteLine("¾"); //This thing will print out as _. So index is correct.
                        }
                    }
                    //Increase the position num
                    position = termsIsAdded ? position + 1 : position;

                    //Keep track of vocabularies for K-gram
                    foreach (string term in ((NormalTokenProcessor)processor).ProcessToken(token))
                    {
                        unstemmedVocabulary.Add(term);
                    }
                }

                //calculate L_{d} for the document and store it index so that we can write it to disk later
                index.CalculateDocWeight();

                //Add author to SoundEx Index
                new SoundEx(Indexer.path).AddDocIdByAuthor(doc.Author, doc.DocumentId, soundEx);
                stream.Dispose();

            }
            new KGram(Indexer.path).buildKGram(unstemmedVocabulary);
            new SoundEx(Indexer.path).BuildSoundexIndex(soundEx);
            index.Save();
            elapsedTime.Stop();
            Console.WriteLine("[Indexer] Done Indexing! Time Elapsed " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
            return index;
        }

    }
}
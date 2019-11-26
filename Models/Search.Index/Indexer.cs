
using Search.Document;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Timers;

namespace Search.Index
{
    public class Indexer
    {

        public static string path = "./";

        public static double averageDocLength;


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
            DiskPositionalIndex index = new DiskPositionalIndex(Indexer.path);
            DiskSoundEx soundEx = new DiskSoundEx(Indexer.path);
            DiskKGram kGram = new DiskKGram(Indexer.path);

            index.Clear();
            soundEx.Clear();
            kGram.Clear();

            ITokenProcessor processor = new StemmingTokenProcesor();

            HashSet<string> unstemmedVocabulary = new HashSet<string>();
            // Index the document
            foreach (IDocument doc in corpus.GetDocuments())
            {
                //Tokenize the documents
                ITokenStream stream = new EnglishTokenStream(doc.GetContent());

                IEnumerable<string> tokens = stream.GetTokens();
                
                //keeptrack of tokens per document 
                int tokenCount =0;

                //keep track of file size 
                int position = 0;
             
                foreach (string token in tokens)
                {
                    tokenCount++;
                    //Process token to term
                    List<string> terms = processor.ProcessToken(token);
                    //Add term to the index
                    bool termsIsAdded = false;
                    foreach (string term in terms)
                    {
                        if (term.Length > 0)
                        {
                            index.AddTerm(term, doc.DocumentId, position);
                            termsIsAdded = true;
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

                //Add token count per document
                index.AddTokensPerDocument(doc.DocumentId, tokenCount);

                //get number of bytes in file 
                string docFilePath = doc.FilePath;
                int fileSizeInByte = (int)(new FileInfo(docFilePath).Length / 8f);
                index.AddByteSize(doc.DocumentId, fileSizeInByte);


                //calculates Average term Frequency for a specific document
                index.CalcAveTermFreq(doc.DocumentId);

                //calculate L_{d} for the document and store it index so that we can write it to disk later
                index.CalculateDocWeight(doc.DocumentId);

                Indexer.averageDocLength = index.calculateAverageDocLength();

                //Add author to SoundEx Index
                soundEx.AddDocIdByAuthor(doc.Author, doc.DocumentId);
                stream.Dispose();

            }
            kGram.buildKGram(unstemmedVocabulary);
            index.Save();
            soundEx.Save();
            elapsedTime.Stop();
            Console.WriteLine("[Indexer] Done Indexing! Time Elapsed " + elapsedTime.Elapsed.ToString("mm':'ss':'fff"));
            GC.Collect();
            return index;
        }

    }
}
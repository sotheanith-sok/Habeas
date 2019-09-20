using Search.Document;
using Search.Index;
using Search.PositionalInvertedIndexer;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Program
{

    public class Program
    {
        public static void Main(string[] args)
        {
            string _directory = "./corpus";
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(_directory);
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();
            PositionalInvertedIndex index =  PositionalInvertedIndexer.IndexCorpus(corpus);
            elapsedTime.Stop();
            Console.WriteLine("Elapsed = {0}", elapsedTime.Elapsed);
            string query;
            IList<Posting> postings;

            while (true)
            {
                Console.Write("\nSearch: ");
                query = Console.ReadLine();

                if (query == ":q")
                {
                    break;
                }
                else if (query == ":vocab")
                {
                    PositionalInvertedIndexer.PrintVocab(index.GetVocabulary(), 100);
                }
                else
                {
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


    }
}

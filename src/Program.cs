using Search.Document;
using Search.Index;
using Search.PositionalInvertedIndexer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


namespace Program
{


    class Program
    {

        public static void Main(string[] args)
        {
            
            //IDocumentCorpus corpus = askDirectory();

            string _directory = "./corpus";
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(_directory);
   
            if (corpus != null && corpus.CorpusSize != 0)
            {

 
                PositionalInvertedIndex index = positIndex(corpus);


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

        ///<summary>
        ///requests Directory/Folder path from user and creates corpus based off that directory
        ///</summary>
        public static IDocumentCorpus askDirectory()
        {
            string _directory;
            Console.WriteLine("Enter the path of Folder or Directory: ");
            _directory = Console.ReadLine();
            if (Directory.Exists(_directory))
            {
                return DirectoryCorpus.LoadTextDirectory(_directory);
            }

            return null;

        }


        ///<summary>
        ///implements stopwatch to measure how long it takes to index corpus
         ///</summary>
        public static PositionalInvertedIndex positIndex(IDocumentCorpus corpus)
        {
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();
            PositionalInvertedIndex index = PositionalInvertedIndexer.IndexCorpus(corpus);
            elapsedTime.Stop();
            Console.WriteLine("Elapsed = {0}", elapsedTime.Elapsed);

            return index;

        }


    }


}

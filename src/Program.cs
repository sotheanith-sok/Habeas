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
        private static PositionalInvertedIndex index;
        private static IDocumentCorpus corpus;

        public static void Main(string[] args)
        {
            //requesting for Directory path from user 
            //IDocumentCorpus corpus = askDirectory();

            string _directory = "./corpus";
            corpus = DirectoryCorpus.LoadTextDirectory(_directory);
   
            if (corpus != null && corpus.CorpusSize != 0)
            {



                // implements stopwatch to measure how long it takes to index corpus
                index = positIndex(corpus);


                string query;
                IList<Posting> postings;

                while (true)
                {
                    Console.Write("\nSearch: ");
                    query = Console.ReadLine();

                    //special queries
                    if (query.StartsWith(":")) {
                        PerformSpecialQueries(query);
                    }
                    //search queries
                    else
                    {
                        postings = index.GetPostings(query);
                        //Print the documents (posting list)
                        PrintPostings(postings);

                        //Ask a document to view and print the content
                        AskDocument(postings);
                    }

                }

            }
            
        }


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

        public static PositionalInvertedIndex positIndex(IDocumentCorpus corpus)
        {
            Stopwatch elapsedTime = new Stopwatch();
            elapsedTime.Start();
            PositionalInvertedIndex index = PositionalInvertedIndexer.IndexCorpus(corpus);
            elapsedTime.Stop();
            Console.WriteLine("Elapsed = {0}", elapsedTime.Elapsed);

            return index;

        }

        /// <summary>
        /// Perform special queries that start with ':'
        /// such as ':q', ':vocab', ':stem', ':index'
        /// </summary>
        /// <param name="specialQuery">a special query to be performed</param>
        public static void PerformSpecialQueries(string specialQuery){
            specialQuery = specialQuery.ToLower();

            if (specialQuery == ":q") {
                System.Environment.Exit(1); // Exit the console app
            }
            else if (specialQuery == ":vocab") {
                PositionalInvertedIndexer.PrintVocab(index.GetVocabulary(), 100);
            }
            // TODO: add speical query actions from Jesse's code

            else {
                Console.WriteLine("No such special query exist.");
                Console.WriteLine(":q             exit the program");
                Console.WriteLine(":stem [token]  print the stemmed token");
                Console.WriteLine(":index [dir]   index a folder");
                Console.WriteLine(":vocab         print vocabulary of the current corpus");
            }
        }

        /// <summary>
        /// Search query from index and
        /// Print the name and the number of documents that contain the term(query)
        /// </summary>
        /// <param name="query">search query</param>
        public static void PerformSearch(string query) {
            if(query.StartsWith(':')) {
                return;
            }

            IList<Posting> postings = index.GetPostings(query);
            PrintPostings(postings);

        }

        /// <summary>
        /// Print the name and count of the documents from a posting list
        /// </summary>
        /// <param name="postings">postings to be printed</param>
        public static void PrintPostings(IList<Posting> postings)
        {
            int i = 1;
            foreach (Posting p in postings)
            {
                IDocument doc = corpus.GetDocument(p.DocumentId);
                Console.Write($"[{i}] {doc.Title} \t{p.Positions.Count} terms");
                Console.Write($"\t\t{p.ToString()}");
                Console.WriteLine();
                i += 1;
            }
            Console.WriteLine($"Found in {postings.Count} files.\n");
        }
        
        /// <summary>
        /// Ask the user the document name and print the content of it
        /// </summary>
        /// <param name="postings">posting list to search the selected document from</param>
        public static void AskDocument(IList<Posting> postings)
        {
            int selected;
            IDocument selectedDocument;
            
            //Ask if the user want to see a doc
            Console.Write("Do you like to view a document? [Y/N]: ");
            Boolean doesWantToView = Console.ReadLine().ToLower().Equals("y");
            if (doesWantToView) {
                //Ask the doc name
                while(true) {
                    Console.Write("Select the document to view (Enter number): ");

                    string input = Console.ReadLine();
                    try {
                        selected = Int32.Parse(input);
                    } catch {
                        continue;
                    }
                    //Console.WriteLine($"selected: {selected}");

                    Boolean isSelectedInRange = (selected > 0) && (selected <= postings.Count);
                    if(!isSelectedInRange) {
                        continue;
                    }

                    selectedDocument = corpus.GetDocument(postings[selected-1].DocumentId);

                    //TODO: Print the content of the doc
                    Console.WriteLine($"\n{selectedDocument.Title.ToUpper()}");
                    Console.WriteLine($"content...\n");
                    //TextReader content = selectedDocument.GetContent();

                    return; //NOTE: It can ask another document to view. But then, when to stop asking??
                }
            } else {
                return;
            }
            //NOTE: handling exception first better? or main action first better?

            
        }
    }


}

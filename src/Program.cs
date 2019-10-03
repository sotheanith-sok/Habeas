using Search.Document;
using Search.Index;
using Search.PositionalInvertedIndexer;
using Search.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Program
{
    class Program
    {
        private static SoundExIndex soundIndex;
        private static PositionalInvertedIndex index;
        private static IDocumentCorpus corpus;

        public static void Main(string[] args)
        {
            
            IDocumentCorpus corpus = AskDirectory();
   
            if (corpus != null && corpus.CorpusSize != 0)
            {
                index = PositIndex(corpus);
                soundIndex = new SoundExIndex(corpus);

                string query;
                IList<Posting> postings;

                while (true)
                {
                    Console.Write("Search: ");
                    query = Console.ReadLine();
                    if (query.Equals("")) {
                        continue;
                    }

                    //special queries
                    if (query.StartsWith(":")) {
                        PerformSpecialQueries(query);
                        continue;
                    }
                    //search queries
                    else
                    {
                        postings = index.GetPostings(query);
                        if (postings.Count > 0) {
                            //Print the documents (posting list)
                            PrintPostings(postings);
                            //Ask a document to view and print the content
                            AskDocument(postings);
                        } else {
                            Console.WriteLine("Not Found.");
                        }
                    }

                }

            }
            
        }

       
        ///<summary>
        ///requests Directory/Folder path from user and creates corpus based off that directory
        ///</summary>
        public static IDocumentCorpus AskDirectory()
        {
            while(true){
            string _directory;
            //Habeas Corpus!!!
            Console.WriteLine("Enter the path of the Folder or Directory you wish to search: ");
            _directory = Console.ReadLine();
            if (Directory.Exists(_directory))
            {
               corpus = DirectoryCorpus.LoadTextDirectory(_directory);
               if(corpus != null && corpus.CorpusSize != 0){
                   return corpus;
               }
               else{
                   Console.WriteLine("Error: the selected path is empty...");
               }
            }
               else{
                   Console.WriteLine("Error: the selected path is invalid...");
               }
            }
        }


        ///<summary>
        ///implements stopwatch to measure how long it takes to index corpus
         ///</summary>
        public static PositionalInvertedIndex PositIndex(IDocumentCorpus corpus)
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
            if(!specialQuery.StartsWith(":")) {
                return;
            }
            specialQuery = specialQuery.ToLower();

            if (specialQuery == ":q") {
                System.Environment.Exit(1); // Exit the console app
            }
            else if (specialQuery.StartsWith(":stem ")) {
                string term = specialQuery.Substring(":stem ".Length);
                Console.WriteLine(new BetterTokenProcessor().StemWords(term));
                Console.WriteLine();
            }
            else if (specialQuery == ":vocab") {
                PositionalInvertedIndexer.PrintVocab(index.GetVocabulary(), 100);
            }
            else if(specialQuery.StartsWith(":index ")){
                corpus = DirectoryCorpus.LoadTextDirectory(specialQuery.Substring(":index ".Length));
                index = PositIndex(corpus);
            }

            else {
                Console.WriteLine("No such special query exist.");
                Console.WriteLine(":q             exit the program");
                Console.WriteLine(":stem [token]  print the stemmed token");
                Console.WriteLine(":index [dir]   index a folder");
                Console.WriteLine(":vocab         print vocabulary of the current corpus");
            }
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
                Console.Write($"    [{i}] {doc.Title} \t{p.Positions.Count} terms");
                Console.Write($"\t\t{p.ToString()}");
                Console.WriteLine();
                i += 1;
            }
            Console.WriteLine($"Found in {postings.Count} files.");
        }
        
        /// <summary>
        /// Ask the user the document to view and print the content of it
        /// </summary>
        /// <param name="postings">posting list to search the selected document from</param>
        public static void AskDocument(IList<Posting> postings)
        {
            int selected;
            IDocument selectedDocument;
            
            //Ask user a document to view
            while(true) {
                Console.Write("Select the number to view the document ([Enter] to exit): ");
                string input = Console.ReadLine();
                //Enter to exit
                if (input.Equals("")) {
                    break;
                }
                //Take number or ask again if input is not numeric
                try {
                    selected = Int32.Parse(input);
                } catch {
                    continue;
                }
                //Console.WriteLine($"selected: {selected}");
                
                //Ask again if input number is not in range
                Boolean isSelectedInRange = (selected > 0) && (selected <= postings.Count);
                if(!isSelectedInRange) {
                    continue;
                }

                selectedDocument = corpus.GetDocument(postings[selected-1].DocumentId);

                PrintContent(selectedDocument);
                return;
            }

        }

        /// <summary>
        /// Print the content of a document.
        /// </summary>
        /// <param name="doc">document to be printed</param>
        public static void PrintContent(IDocument doc) {
            Console.WriteLine($"\n{doc.Title.ToUpper()}");
            TextReader content = doc.GetContent();
            Console.WriteLine(content.ReadToEnd());
            
        }
    }


}

using Search.Document;
using Search.Index;
using Search.PositionalInvertedIndexer;
using Search.Text;
using System;
using System.Collections.Generic;
using System.IO;

namespace Program
{
    class Program
    {
        private static PositionalInvertedIndex index;
        private static IDocumentCorpus corpus;

        public static void Main(string[] args)
        {
            
            IDocumentCorpus corpus = AskDirectory();
   
            if (corpus != null && corpus.CorpusSize != 0)
            {
                index = PositionalInvertedIndexer.IndexCorpus(corpus);

                string query;
                IList<Posting> postings;

                while (true)
                {
                    Console.Write("\nSearch: ");
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
                        //TODO: Use BooleanQueryParser
                        postings = index.GetPostings(query);
                        if (postings.Count > 0) {
                            //Print the documents (posting list)
                            PrintPostings(postings);
                            //Ask a document to view and print the content
                            IDocument doc = AskDocumentToView(postings);
                            PrintContent(doc);
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

        /// <summary>
        /// Perform special queries that start with ':'
        /// such as ':q', ':vocab', ':stem', ':index', ':help'
        /// </summary>
        /// <param name="specialQuery">a special query to be performed</param>
        public static void PerformSpecialQueries(string specialQuery){
            string info_support = "1. single query             Y\n"
                                + "2. boolean query            N  space for AND, + for OR\n"
                                + "3. phrase query             N  \"term1 term2 ...\"\n"
                                + "4. near query               N  [term1 NEAR/k term2]\n"
                                + "5. wildcard queryv          N  colo*r\n"
                                + "6. soundex for author name  N";
            string info_special = ":q             exit the program\n"
                                + ":stem [token]  print the stemmed token\n"
                                + ":index [dir]   index a folder\n"
                                + ":vocab         print vocabulary of the current corpus\n"
                                + ":h, :help      help";

            if(!specialQuery.StartsWith(":")) {
                return;
            }
            specialQuery = specialQuery.ToLower();

            if (specialQuery == ":q") {                                             // :q
                System.Environment.Exit(1); // Exit the console app
            }
            else if (specialQuery.StartsWith(":stem ")) {                           // :stem
                string term = specialQuery.Substring(":stem ".Length);
                Console.WriteLine( new BetterTokenProcessor().StemWords(term) );
            }
            else if (specialQuery == ":vocab") {                                    // :vocab
                PrintVocab(index, 1000);
            }
            else if(specialQuery.StartsWith(":index ")) {                           // :index
                string directory = specialQuery.Substring(":index ".Length);
                corpus = DirectoryCorpus.LoadTextDirectory(directory);
                index = PositionalInvertedIndexer.IndexCorpus(corpus);
            }
            else if( (specialQuery == ":help") || (specialQuery == ":h") ) {        // :help
                Console.WriteLine("This search engine supports\n" + info_support);
                Console.WriteLine("\nSpecial queries\n" + info_special);
            }
            else {
                Console.WriteLine("No such special query exist.");
                Console.WriteLine(info_special);
            }
        }

        /// <summary>
        /// Prints the sorted vocabulary of an index
        /// </summary>
        /// <param name="index">the index to print a sorted vocabulary from</param>
        /// <param name="count">the number of terms to print</param>
        public static void PrintVocab(IIndex index, int count)
        {
            IReadOnlyList<string> vocabulary = index.GetVocabulary();
            for (int i = 0; i < Math.Min(count, vocabulary.Count); i++)
            {
                Console.WriteLine(vocabulary[i]);
            }
            Console.WriteLine($"Total: {vocabulary.Count} terms");
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
        /// Ask the user the document to view
        /// </summary>
        /// <param name="postings">posting list to search the selected document from</param>
        /// <return>a selected document to view</return>
        public static IDocument AskDocumentToView(IList<Posting> postings)
        {
            int selected;
            IDocument selectedDocument;
            string input;
            
            //Ask user a document to view
            while(true) {
                Console.Write("Select the number to view the document ([Enter] to exit): ");
                input = Console.ReadLine();
                //Enter to exit
                if (input.Equals("")) {
                    selectedDocument = null;
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
                if(isSelectedInRange) {
                    selectedDocument = corpus.GetDocument(postings[selected-1].DocumentId);
                    break;
                }
            }
            return selectedDocument;
        }

        /// <summary>
        /// Print the content of a document.
        /// </summary>
        /// <param name="doc">document to be printed</param>
        public static void PrintContent(IDocument doc) {
            if(doc != null) {
                Console.WriteLine($"\n{doc.Title.ToUpper()}");
                TextReader content = doc.GetContent();
                Console.WriteLine(content.ReadToEnd());
            }
        }
    }


}

using Search.Document;
using Search.Index;
using Search.Query;
using Search.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;

namespace Program
{
    class Program
    {
        private static SoundExIndex soundIndex; //for author
        private static PositionalInvertedIndex index;   //for body
        private static IDocumentCorpus corpus;
        private static Indexer indexer;

        public static void Main(string[] args)
        {
            string query;
            IList<Posting> postings;
            IQueryComponent component;
            BooleanQueryParser parser = new BooleanQueryParser();
            ITokenProcessor processor = new StemmingTokenProcesor();

            AssemblyName appName = Assembly.GetEntryAssembly().GetName();
            string projectName = appName.Name;
            string projectVersion = appName.Version.Major.ToString()
                              + '.' + appName.Version.Minor.ToString();
            Console.WriteLine($"[{projectName} {projectVersion}]");

            corpus = GetCorpusByAskingDirectory();
            
            //Use indexer to build the main index and soundexIndex
            indexer = new Indexer();
            index = indexer.IndexCorpus(corpus);
            soundIndex = indexer.SoundIndex;

            while (true)
            {
                //get query input from user
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
                else {
                    component = parser.ParseQuery(query);
                    postings = component.GetPostings(index, processor);
                    if (postings.Count > 0) {
                        PerformSearchResult(postings, corpus, false, true, false);
                    } else {
                        Console.WriteLine("Not Found.");
                    }

                }
            }

        }

       
        ///<summary>
        ///Requests directory path from user and creates corpus based off that directory
        ///</summary>
        private static IDocumentCorpus GetCorpusByAskingDirectory()
        {
            IDocumentCorpus corpus;
            string directory;

            while (true)
            {
                //Habeas Corpus!!! lol
                Console.WriteLine("Enter the path of the directory you wish to search: ");
                directory = Console.ReadLine();

                if (!Directory.Exists(directory)) {
                    Console.WriteLine("The directory doesn't exist.");
                    continue;
                }

                //Build corpus
                corpus = DirectoryCorpus.LoadTextDirectory(directory);

                if (corpus == null || corpus.CorpusSize == 0) {
                    Console.WriteLine("The directory is empty.");
                    continue;
                }
                else {
                    // when valid corpus is constructed
                    break;
                }
            }

            return corpus;
        }

        /// <summary>
        /// Performs special queries that start with ':'
        /// such as ':q', ':vocab', ':stem', ':index', ':help'
        /// </summary>
        /// <param name="specialQuery">a special query to be performed</param>
        private static void PerformSpecialQueries(string specialQuery)
        {
            string info_support = "1. single query        Y\n"
                                + "2. boolean query       Y  space for AND, + for OR\n"
                                + "3. phrase query        Y  \"term1 term2 ...\"\n"
                                + "4. near query          Y  [term1 NEAR/k term2]\n"
                                + "5. wildcard query      Y  colo*r\n"
                                + "6. author soundex      Y  :author name";
            string info_special = ":q             exit the program\n"
                                + ":stem [token]  print the stemmed token\n"
                                + ":index [dir]   index a folder\n"
                                + ":vocab         print vocabulary of the current corpus\n"
                                + ":author [name] find documents by similar-sounding authors"
                                + ":h, :help      help";

            if (!specialQuery.StartsWith(":"))
            {
                return;
            }
            specialQuery = specialQuery.ToLower();

            if (specialQuery == ":q") {                                             // :q
                System.Environment.Exit(1); // Exit the console app
            }
            else if (specialQuery.StartsWith(":stem ")) {                           // :stem
                string term = specialQuery.Substring(":stem ".Length);
                Console.WriteLine(new StemmingTokenProcesor().StemWords(term));
                Console.WriteLine();
            }
            else if (specialQuery == ":vocab") {                                    // :vocab
                PrintVocab(index, 1000);
            }
            else if (specialQuery.StartsWith(":index ")) {                          // :index
                string directory = specialQuery.Substring(":index ".Length);
                
                if (!Directory.Exists(directory)) {
                    Console.WriteLine("The directory doesn't exist.");
                    return;
                }
                if (corpus == null || corpus.CorpusSize == 0) {
                    Console.WriteLine("The directory is empty.");
                    return;
                }
                corpus = DirectoryCorpus.LoadTextDirectory(directory);
                index = indexer.IndexCorpus(corpus);
            }
            else if (specialQuery.StartsWith(":author ")) {                         // :author
                string name = specialQuery.Substring(":author ".Length);
                IList<Posting> postings = soundIndex.GetPostings(name);
                PerformSearchResult(postings, corpus, true, false, false);
            }
            else if ((specialQuery == ":help") || (specialQuery == ":h")) {         // :help
                Console.WriteLine("This search engine supports\n" + info_support);
                Console.WriteLine("\nSpecial queries\n" + info_special);
            }
            else
            {
                Console.WriteLine("No such special query exist.");
                Console.WriteLine(info_special);
            }
        }

        /// <summary>
        /// Prints the sorted vocabulary of an index
        /// </summary>
        /// <param name="index">the index to print a sorted vocabulary from</param>
        /// <param name="count">the number of terms to print</param>
        private static void PrintVocab(IIndex index, int count)
        {
            IReadOnlyList<string> vocabulary = index.GetVocabulary();
            int minCount = Math.Min(count, vocabulary.Count);
            for (int i = 0; i < minCount; i++)
            {
                Console.WriteLine(vocabulary[i]);
            }
            Console.WriteLine($"Total: {vocabulary.Count} terms");
        }

        /// <summary>
        /// Performs after-search actions including
        /// 1) printing the search result
        /// 2) asking user which document to view
        /// 3) printing the content
        /// </summary>
        /// <param name="postings"></param>
        /// <param name="corpus">the corpus to get result from</param>
        /// <param name="author">option to print author info</param>
        /// <param name="termCount">option to print term frequency of each document</param>
        /// <param name="details">option to print detail(docID, positions)</param>
        private static void PerformSearchResult(IList<Posting> postings, IDocumentCorpus corpus, bool author, bool termCount, bool details) {
            PrintPostings(postings, corpus, author, termCount, details);
            IDocument doc = AskDocumentToView(postings, corpus);
            PrintContent(doc);
        }
        /// <summary>
        /// Prints the name and count of the documents from a posting list
        /// </summary>
        /// <param name="postings">postings to be printed</param>
        /// <param name="corpus">corpus to get the document title from</param>
        /// <param name="author">option to print author info</param>
        /// <param name="termCount">option to print term frequency of each document</param>
        /// <param name="details">option to print detail(docID, positions)</param>
        private static void PrintPostings(IList<Posting> postings, IDocumentCorpus corpus, bool bAuthor, bool bTermCount, bool bDetails)
        {
            //Print header
            if(bAuthor == true) {
                Console.WriteLine($"    NUM  AUTHOR\t\tTITLE");
            } else {
                Console.WriteLine($"    NUM  TITLE");
            }

            //Print postings
            int i = 1;
            foreach (Posting p in postings)
            {
                IDocument doc = corpus.GetDocument(p.DocumentId);
                Console.Write($"    [{i}]  ");
                if(bAuthor)    { Console.Write($"{doc.Author}\t"); }
                if( doc.Title == "" || doc.Title == null) {
                    Console.Write("No Title");
                } else {
                    Console.Write($"{doc.Title}");
                }
                if(bTermCount) { Console.Write($"\t{p.Positions.Count} terms"); }
                if(bDetails)   { Console.Write($"\t\t{p.ToString()}"); }
                Console.WriteLine();
                i += 1;
                ((IDisposable)doc).Dispose();
            }
            Console.WriteLine($"Found {postings.Count} files.");
        }

        /// <summary>
        /// Asks the user the document to view
        /// </summary>
        /// <param name="postings">posting list to search the selected document from</param>
        /// <param name="corpus">corpus to get the document content from</param>
        /// <return>a selected document to view</return>
        private static IDocument AskDocumentToView(IList<Posting> postings, IDocumentCorpus corpus)
        {
            int selected;
            IDocument selectedDocument;
            string input;

            //Ask user a document to view
            while (true)
            {
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
                if (isSelectedInRange) {
                    selectedDocument = corpus.GetDocument(postings[selected - 1].DocumentId);
                    break;
                }
            }
            return selectedDocument;
        }

        /// <summary>
        /// Prints the content of a document.
        /// </summary>
        /// <param name="doc">document to be printed</param>
        private static void PrintContent(IDocument doc)
        {
            if (doc != null)
            {
                Console.WriteLine($"\n{doc.Title.ToUpper()}");
                TextReader content = doc.GetContent();
                Console.WriteLine(content.ReadToEnd());
                content.Dispose();
                ((IDisposable)doc).Dispose();
            }
        }

    }


}

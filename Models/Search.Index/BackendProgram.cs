using Search.Document;
using Search.Index;
using Search.Query;
using Search.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Search.Index
{
    public class BackendProgram
    {
        private static SoundExIndex soundIndex;
        private static PositionalInvertedIndex index;
        private static IDocumentCorpus corpus;

        public void GenerateIndex(string check) {
            Console.WriteLine("");
            BooleanQueryParser parser = new BooleanQueryParser();
            ITokenProcessor processor = new StemmingTokenProcesor();
            AssemblyName appName = Assembly.GetEntryAssembly().GetName();
            string projectName = appName.Name;
            string projectVersion = appName.Version.Major.ToString()
                              + '.' + appName.Version.Minor.ToString();

            corpus = DirectoryCorpus.LoadTextDirectory(check);
            Console.WriteLine("28");

            if (corpus != null && corpus.CorpusSize != 0)   //NOTE: redundant..
            {
                index = PositionalInvertedIndexer.IndexCorpus(corpus);
                soundIndex = new SoundExIndex(corpus);


            }
            Console.WriteLine("37");
        }

        /// <summary>
        /// Returns postings for soundex query
        /// </summary>
        public List<string> soundexTerm(string term)
        {
            string name = term;
            IList<Posting> postings = soundIndex.GetPostings(name);
            List<String> results = new List<string>();
            if (postings.Count > 0)
            {
                results.Add(postings.Count.ToString());
                foreach (Posting p in postings)
                {
                    IDocument doc = corpus.GetDocument(p.DocumentId);
                    results.Add(doc.Title);
                    results.Add(doc.DocumentId.ToString());
                }
            }
            else
            {
                results.Add("0");
            }
            return results;
        }

        /// <summary>
        /// Returns postings for a query
        /// </summary>
        public List<string> searchTerm(string term)
        {

            List<String> results = new List<string>();
            IList<Posting> postings;
            IQueryComponent component;
            BooleanQueryParser parser = new BooleanQueryParser();
            ITokenProcessor processor = new StemmingTokenProcesor();
            component = parser.ParseQuery(term);
            postings = component.GetPostings(index, processor);
            if (postings.Count > 0)
            {
                results.Add(postings.Count.ToString());
                foreach (Posting p in postings) {
                    IDocument doc = corpus.GetDocument(p.DocumentId);
                    results.Add(doc.Title);
                    results.Add(doc.DocumentId.ToString());
                }
            }
            else
            {
                results.Add("0");
            }
            return results;
        }

        /// <summary>
        /// Returns stemmed version of a string
        /// </summary>
        public string termStemmer(string term)
        {
            string result = new StemmingTokenProcesor().StemWords(term);
            Console.WriteLine(result);
            return result;
        }

        /// <summary>
        /// Returns true if the indicated path exists
        /// </summary>
        public bool PathIsValid(string CandidatePath)
        {

            if (Directory.Exists(CandidatePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the indicated path contains content
        /// </summary>
        public bool PathContainsContent(string CandidatePath)
        {

            corpus = DirectoryCorpus.LoadTextDirectory(CandidatePath);

            if (corpus == null || corpus.CorpusSize == 0)
            {
                return false;
            }
            else { return true; }

        }

        ///<summary>
        ///Requests Directory/Folder path from user and creates corpus based off that directory
        ///</summary>
        public static IDocumentCorpus GetCorpusByAskingDirectory()
        {
            IDocumentCorpus corpus;
            string directory;

            while (true)
            {
                //Habeas Corpus!!! lol
                Console.WriteLine("Enter the path of the directory you wish to search: ");
                directory = Console.ReadLine();

                if (!Directory.Exists(directory))
                {
                    Console.WriteLine("The directory doesn't exist.");
                    continue;
                }

                corpus = DirectoryCorpus.LoadTextDirectory(directory);

                if (corpus == null || corpus.CorpusSize == 0)
                {
                    Console.WriteLine("The directory is empty.");
                    continue;
                }
                else
                {
                    // when valid corpus is constructed
                    break;
                }
            }

            return corpus;
        }

        /// <summary>
        /// Perform special queries that start with ':'
        /// such as ':q', ':vocab', ':stem', ':index', ':help'
        /// </summary>
        /// <param name="specialQuery">a special query to be performed</param>
        public static void PerformSpecialQueries(string specialQuery)
        {
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

            if (!specialQuery.StartsWith(":"))
            {
                return;
            }
            specialQuery = specialQuery.ToLower();

            if (specialQuery == ":q")
            {                                             // :q
                System.Environment.Exit(1); // Exit the console app
            }
            else if (specialQuery.StartsWith(":stem "))
            {                           // :stem
                string term = specialQuery.Substring(":stem ".Length);
                Console.WriteLine(new StemmingTokenProcesor().StemWords(term));
                Console.WriteLine();
            }
            else if (specialQuery == ":vocab")
            {                                    // :vocab
                PrintVocab(index, 1000);
            }
            else if (specialQuery.StartsWith(":index "))
            {                           // :index
                string directory = specialQuery.Substring(":index ".Length);

                if (!Directory.Exists(directory))
                {
                    Console.WriteLine("The directory doesn't exist.");
                    return;
                }
                if (corpus == null || corpus.CorpusSize == 0)
                {
                    Console.WriteLine("The directory is empty.");
                    return;
                }
                corpus = DirectoryCorpus.LoadTextDirectory(directory);
                index = PositionalInvertedIndexer.IndexCorpus(corpus);
            }
            else if ((specialQuery == ":help") || (specialQuery == ":h"))
            {        // :help
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
        public List<String> PrintVocab(int count)
        {
            return PrintVocab(index, count);
        }

        /// <summary>
        /// Prints the sorted vocabulary of an index
        /// </summary>
        /// <param name="index">the index to print a sorted vocabulary from</param>
        /// <param name="count">the number of terms to print</param>
        public static List<String> PrintVocab(IIndex index, int count)
        {
            List<String> nVocab = new List<string>();
            IReadOnlyList<string> vocabulary = index.GetVocabulary();
            nVocab.Add(vocabulary.Count.ToString());
            for (int i = 0; i < Math.Min(count, vocabulary.Count); i++)
            {
                nVocab.Add(vocabulary[i]);
            }
            return nVocab;
        }

        //private static void PrintPostings(IList<Posting> postings, IDocumentCorpus corpus, bool bAuthor, bool bTermCount, bool bDetails)
        //{
        //    //Print header
        //    if (bAuthor == true)
        //    {
        //        Console.WriteLine($"    NUM  AUTHOR\t\tTITLE");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"    NUM  TITLE");
        //    }

        //    //Print postings
        //    int i = 1;
        //    foreach (Posting p in postings)
        //    {
        //        IDocument doc = corpus.GetDocument(p.DocumentId);
        //        Console.Write($"    [{i}]  ");
        //        if (bAuthor) { Console.Write($"{doc.Author}\t"); }
        //        if (doc.Title == "" || doc.Title == null)
        //        {
        //            Console.Write("No Title");
        //        }
        //        else
        //        {
        //            Console.Write($"{doc.Title}");
        //        }
        //        if (bTermCount) { Console.Write($"\t{p.Positions.Count} terms"); }
        //        if (bDetails) { Console.Write($"\t\t{p.ToString()}"); }
        //        Console.WriteLine();
        //        i += 1;
        //    }
        //    Console.WriteLine($"Found {postings.Count} files.");
        //}

        public string getDocContent(string doc)
        {
            string finalString;
            int selected = Int32.Parse(doc);
            //Console.WriteLine(selected);
            IDocument selectedDocument;
            selectedDocument = corpus.GetDocument(selected);
            //finalString.($"\n{doc.Title.ToUpper()}");
            TextReader content = selectedDocument.GetContent();
            finalString = (content.ReadToEnd());
            content.Dispose();
            ((IDisposable)selectedDocument).Dispose();

            return finalString;
        }

        public string getDocTitle(string doc)
        {
            string finalString;
            int selected = Int32.Parse(doc);
            IDocument selectedDocument;
            selectedDocument = corpus.GetDocument(selected);
            finalString = selectedDocument.Title;
    
            return finalString;
        }

        /// <summary>
        /// Ask the user the document to view
        /// </summary>
        /// <param name="postings">posting list to search the selected document from</param>
        /// <param name="corpus">corpus to get the document content from</param>
        /// <return>a selected document to view</return>
        public static IDocument AskDocumentToView(IList<Posting> postings, IDocumentCorpus corpus)
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
                if (input.Equals(""))
                {
                    selectedDocument = null;
                    break;
                }
                //Take number or ask again if input is not numeric
                try
                {
                    selected = Int32.Parse(input);
                }
                catch
                {
                    continue;
                }
                //Console.WriteLine($"selected: {selected}");

                //Ask again if input number is not in range
                Boolean isSelectedInRange = (selected > 0) && (selected <= postings.Count);
                if (isSelectedInRange)
                {
                    selectedDocument = corpus.GetDocument(postings[selected - 1].DocumentId);
                    break;
                }
            }
            return selectedDocument;
        }

        /// <summary>
        /// Print the content of a document.
        /// </summary>
        /// <param name="doc">document to be printed</param>
        public static void PrintContent(IDocument doc)
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

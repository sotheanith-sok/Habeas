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
        private static IIndex index; //currently set-up to use on-disk index
        private static IDocumentCorpus corpus;

        //mode indicates if the search engine is in boolean mode or ranked retrieval mode
        //if mode is true, the search engine is in boolean mode
        //if mode is false, it's in ranked retrieval mode
        private static Boolean mode = true;

        /// <summary>
        /// Gets a corpus
        /// </summary>
        /// <param name="path">the selected directory path</param>
        public void GetIndex(string path)
        {
            string binFiles = path + "\\index\\";
            //if bin files exist, done
            if (Directory.Exists(binFiles))
            {
                index = new DiskPositionalIndex(binFiles);
                corpus = DirectoryCorpus.LoadTextDirectory(path);
                return;
            }
            //else
            else
            {
                GenerateIndex(path);
            }
        }

        /// <summary>
        /// Gets on-disk index or generate a new index out of the selected corpus
        /// </summary>
        /// <param name="path">the path to the selected corpus</param>
        public void GetIndex(string path)
        {
            string pathToIndex = path + "/index/";
            bool doesOnDiskIndexExist = Directory.Exists(pathToIndex);
            // bool doesOnDiskIndexExist = Directory.Exists(pathToIndex) && (Directory.GetFiles(pathToIndex).Length != 0);
            
            corpus = DirectoryCorpus.LoadTextDirectory(path);

            if (doesOnDiskIndexExist)
            {
                Console.WriteLine("[Index] The on-disk index exists! Reading the on-disk index.");
                index = new DiskPositionalIndex(pathToIndex);
            }
            else
            {
                Console.WriteLine("[Index] Generating new index.");
                GenerateIndex(path);
            }
        }

        /// <summary>
        /// Indexes the corpus and writes the generated index
        /// </summary>
        /// <param name="path">the path to the selected corpus</param>
        private void GenerateIndex(string path)
        {
            //make corpus out of the selected directory path
            // corpus = DirectoryCorpus.LoadTextDirectory(path);

            //if the corpus contains content
            if (corpus != null && corpus.CorpusSize != 0)
            {
                //make an index for the corpus
                PositionalInvertedIndex inMemoryIndex = Indexer.IndexCorpus(corpus);
                //Write the in-memory index on disk.
                DiskIndexWriter diskIndexWriter = new DiskIndexWriter();

                diskIndexWriter.WriteIndex(inMemoryIndex, path);
                //TODO: hide index better (hidden folder)
                index = new DiskPositionalIndex(path + "/index/");
            }
        }

        /// <summary>
        /// Returns postings for soundex query
        /// </summary>
        /// <param name="name">the author name being queried</param>
        public List<string> SearchSoundexQuery(string name)
        {
            //get a list of postings given the name
            IList<Posting> postings = new SoundEx(Indexer.path).GetPostings(name);
            //list of strings to return
            List<String> results = new List<string>();
            //if the query returns any results
            if (postings.Count > 0)
            {
                //add the number of postings to the list of strings to return
                results.Add(postings.Count.ToString());
                //for each posting
                foreach (Posting p in postings)
                {
                    //use the posting's id to access the document
                    IDocument doc = corpus.GetDocument(p.DocumentId);
                    //add the title and name of the author to the list of strings to be returned 
                    results.Add(doc.Title + " (Author: " + doc.Author + ")");
                    //also add the document id to the list of strings to be returned
                    results.Add(doc.DocumentId.ToString());
                }
            }
            else
            {
                //if there are no postings just return a list with a zero in it
                results.Add("0");
            }
            //return the final list of strings
            return results;
        }

        /// <summary>
        /// Returns postings for a query
        /// </summary>
        /// <param name="query">the query which the user is making to the search engine</param>
        public List<string> SearchQuery(string query)
        {
            // Console.Write("Corpus size is:");
            // Console.WriteLine(corpus.CorpusSize);
            if(mode == false){
                //do ranked retrieval
            }
            //the list of strings to return 
            List<String> results = new List<string>();
            //the list of postings
            IList<Posting> postings;
            IQueryComponent component;
            //create a boolean query parser
            BooleanQueryParser parser = new BooleanQueryParser();
            //create a stemming token processor
            ITokenProcessor processor = new StemmingTokenProcesor();
            //parse the query
            component = parser.ParseQuery(query);
            //get the postings
            postings = component.GetPostings(index, processor);
            //if there are any postings...
            if (postings.Count > 0)
            {
                //add the count of the postings to the list of strings to be returned
                results.Add(postings.Count.ToString());
                //for each posting...
                foreach (Posting p in postings)
                {
                    //use the document id to access the document
                    IDocument doc = corpus.GetDocument(p.DocumentId);
                    //add the title to the list of strings to be returned
                    results.Add(doc.Title);
                    //add the document id to the list of strings to be returned 
                    results.Add(doc.DocumentId.ToString());
                }
            }
            //if there aren't any postings...
            else
            {
                //add a zero to the list of strings to be returned
                results.Add("0");
            }
            //return the list of strings
            return results;
        }

        /// <summary>
        /// Returns stemmed version of a string
        /// </summary>
        /// <param name="term">string to be stemmed</param>
        public string StemTerm(string term)
        {
            //send the term into the stemmer
            string result = new StemmingTokenProcesor().StemWords(term);
            //resturn the result
            return result;
        }

        /// <summary>
        /// Returns true if the indicated path exists
        /// </summary>
        /// <param name="path">the directory path chosen by the user</param>
        public bool CheckIfPathValid(string path)
        {
            //if the path exists...
            if (Directory.Exists(path))
            {
                //return true
                return true;
            }
            //if the path does not exist...
            else
            {
                //return false
                return false;
            }
        }

        /// <summary>
        /// switches from boolean mode to rr mode and vice versa
        /// </summary>
        public void switchMode()
        {
            if (mode == true)
            {
                mode = false;
                Console.WriteLine(mode);
            }
            else
            {
                mode = true;
                Console.WriteLine(mode);
            }
        }

        /// <summary>
        /// Returns true if the indicated path contains content
        /// </summary>
        /// <param name="path">the directory path chosen by the user</param>
        public bool CheckIfPathContainsContent(string path)
        {
            //return true if the chosen path contains content
            return Directory.GetFiles(path).Length != 0;

        }


        /// <summary>
        /// Prints the sorted vocabulary of an index
        /// </summary>
        /// <param name="count">the number of vocab terms to be returned</param>
        public List<String> PrintVocab(int count)
        {
            //return a list of strings of all the vocab which the program requires
            return PrintVocab(index, count);
        }

        /// <summary>
        /// Prints the sorted vocabulary of an index
        /// </summary>
        /// <param name="index">the index to print a sorted vocabulary from</param>
        /// <param name="count">the number of terms to print</param>
        public static List<String> PrintVocab(IIndex index, int count)
        {
            //create a list of strings to be returned
            List<String> nVocab = new List<string>();
            //get list of vocab from index
            IReadOnlyList<string> vocabulary = index.GetVocabulary();
            //add vocab count to list of strings to be returned
            nVocab.Add(vocabulary.Count.ToString());
            //for the amount of vocab less than count
            for (int i = 0; i < Math.Min(count, vocabulary.Count); i++)
            {
                //add the vocab to the list of strings to be returned
                nVocab.Add(vocabulary[i]);
            }
            //return the list of strings
            return nVocab;
        }


        /// <summary>
        /// Gets the content of a document
        /// </summary>
        /// <param name="doc">the id of the document in question</param>
        public string GetDocContent(string doc)
        {
            //string to be returned
            string finalString;
            //turn the doc value into an int
            int selected = Int32.Parse(doc);
            //get a document based on the id
            IDocument selectedDocument = corpus.GetDocument(selected);
            //get the content of the document
            TextReader content = selectedDocument.GetContent();
            //set the string to be returned equal to the content of the document
            finalString = (content.ReadToEnd());
            //dispose of the content
            content.Dispose();
            //return the string
            return finalString;
        }

        /// <summary>
        /// Gets the title of a document
        /// </summary>
        /// <param name="doc">the id of the document in question</param>
        public string GetDocTitle(string doc)
        {
            //convert doc into an int
            int selected = Int32.Parse(doc);
            //get document from the document id
            IDocument selectedDocument = corpus.GetDocument(selected);
            //return the title of the document
            return selectedDocument.Title;
        }
    }
}

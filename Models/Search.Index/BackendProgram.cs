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
        private static PositionalInvertedIndex index;
        private static IDocumentCorpus corpus;

        public void GenerateIndex(string check)
        {

            BooleanQueryParser parser = new BooleanQueryParser();

            ITokenProcessor processor = new StemmingTokenProcesor();

            AssemblyName appName = Assembly.GetEntryAssembly().GetName();

            string projectName = appName.Name;

            string projectVersion = appName.Version.Major.ToString()
                              + '.' + appName.Version.Minor.ToString();

            corpus = DirectoryCorpus.LoadTextDirectory(check);

            if (corpus != null && corpus.CorpusSize != 0)   //NOTE: redundant..
            {
                index = Indexer.IndexCorpus(corpus);
            }
        }

        /// <summary>
        /// Returns postings for soundex query
        /// </summary>
        public List<string> soundexTerm(string term)
        {
            string name = term;
            IList<Posting> postings = Indexer.soundEx.GetPostings(name);
            List<String> results = new List<string>();
            if (postings.Count > 0)
            {
                results.Add(postings.Count.ToString());
                foreach (Posting p in postings)
                {
                    IDocument doc = corpus.GetDocument(p.DocumentId);
                    results.Add(doc.Title + " (Author: " + doc.Author + ")");
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
        /// Returns stemmed version of a string
        /// </summary>
        public string termStemmer(string term)
        {
            string result = new StemmingTokenProcesor().StemWords(term);

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

            return Directory.GetFiles(CandidatePath).Length != 0;

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



        public string getDocContent(string doc)
        {
            string finalString;
            int selected = Int32.Parse(doc);

            IDocument selectedDocument;
            selectedDocument = corpus.GetDocument(selected);

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


    }


}

using System.Collections.Generic;
using Search.Text;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.IO;
namespace Search.Index
{
    /// <summary>
    /// K-gram object uses to store and process k-gram requests
    /// </summary>
    public class KGram
    {
        //Map uses to store data internally
        private Dictionary<string, List<string>> map;

        //Map uses to manp k-gram less than size to k-gram
        private Dictionary<string, List<string>> miniMap;

        //size of each k-gram terms.
        public int size { get; }

        //Path to kGrame
        private string path;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies.false Non stem.</param>
        /// <param name="size">Size of each k-gram term</param>
        public KGram(HashSet<string> vocabularies, string path, int size = 3)
        {
            this.map = new Dictionary<string, List<string>>();
            this.size = size;
            this.miniMap = new Dictionary<string, List<string>>();
            this.path = path;
            buildKGram(vocabularies);
        }

        /// <summary>
        /// Construct the k-grams map
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies</param>
        private void buildKGram(HashSet<string> vocabularies)
        {
            Console.WriteLine("Generating K-Gram...");

            //K-gram vocabularies and add them to dictionary
            foreach (string vocab in vocabularies)
            {
                //Split the vocabulary
                List<string> kGrams = this.KGramSplitter("$" + vocab + "$", this.size);

                //Add k-grams to dictionary
                foreach (string kGram in kGrams)
                {
                    if (this.map.ContainsKey(kGram))
                    {
                        this.map[kGram].Add(vocab);
                    }
                    else
                    {
                        this.map.Add(kGram, new List<string> { vocab });
                    }
                }
            }

            //Build lesser k-gram to handle wildcard query lesser than size
            foreach (string kGram in this.map.Keys)
            {
                for (int k = 0; k < this.size; k++)
                {
                    List<string> miniKGrams = this.KGramSplitter(kGram, k);
                    foreach (string miniKGram in miniKGrams)
                    {
                        if (!string.IsNullOrWhiteSpace(miniKGram) && miniKGram != "$")
                        {
                            if (this.miniMap.ContainsKey(miniKGram))
                            {
                                this.miniMap[miniKGram].Add(kGram);
                            }
                            else
                            {
                                this.miniMap.Add(miniKGram, new List<string> { kGram });
                            }
                        }
                    }
                }
            }

            //Print Results
            Console.WriteLine("K-Gram: " + this.map.Keys.Count);
            Console.WriteLine("Mini K-Gram: " + this.miniMap.Keys.Count);

            //WriteKGramToDisk
            Console.WriteLine("Write K-Gram to disk...");
            Console.WriteLine("Path:" + Path.GetFullPath(this.path));
            DiskKGramWriter kGramWriter = new DiskKGramWriter();
            kGramWriter.WriteKGram(this, this.path);

        }

        /// <summary>
        /// Get a list of vocabularies for a given k-gram
        /// </summary>
        /// <param name="kGram">K-gram to search for</param>
        /// <returns>A list of vocabularies</returns>
        public List<string> getVocabularies(string kGram)
        {
            //If requested k-gram's length is less than this k-gram size, use mini kgram to find the right k-gram 
            if (kGram.Length < this.size)
            {
                HashSet<string> candidates = new HashSet<string>();
                List<string> possibleKGram = (this.miniMap.ContainsKey(kGram)) ? this.miniMap[kGram] : new List<string>();
                foreach (string k in possibleKGram)
                {
                    foreach (string v in this.map[k])
                    {
                        candidates.Add(v);
                    }
                }
                return candidates.ToList();
            }
            else
            {
                return (this.map.ContainsKey(kGram)) ? this.map[kGram] : new List<string>();

            }
        }


        /// <summary>
        /// Split k-gram
        /// </summary>
        /// <param name="term">term to be split</param>
        /// <returns> list of kgram</returns>
        private List<string> KGramSplitter(string term, int size)
        {
            if (term.Length < size)
            {
                return new List<string> { term };
            }
            else
            {
                int i = 0;
                List<string> result = new List<string>();
                while (i + size <= term.Length)
                {
                    result.Add(term.Substring(i, size));
                    i++;
                }
                return result;
            }

        }

        /// <summary>
        /// Read internal kGram map
        /// </summary>
        /// <returns>Read only kGram map</returns>
        public IReadOnlyDictionary<string, ReadOnlyCollection<string>> GetKGramMap()
        {
            return (IReadOnlyDictionary<string, ReadOnlyCollection<string>>)this.map.ToDictionary(pair => pair.Key, pair => pair.Value.AsReadOnly());
        }

        /// <summary>
        /// Read internal mini kGram map
        /// </summary>
        /// <returns>Read only mini kGram map</returns>
        public IReadOnlyDictionary<string, ReadOnlyCollection<string>> GetMiniKGramMap()
        {
            return (IReadOnlyDictionary<string, ReadOnlyCollection<string>>)this.miniMap.ToDictionary(pair => pair.Key, pair => pair.Value.AsReadOnly());
        }

    }
}
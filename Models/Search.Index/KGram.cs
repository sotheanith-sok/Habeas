using System.Collections.Generic;
using System;
using System.Linq;
using Search.OnDiskDataStructure;
using System.IO;
namespace Search.Index
{
    /// <summary>
    /// K-gram object uses to store and process k-gram requests
    /// </summary>
    public class KGram
    {
        //size of each k-gram terms.
        public int size { get; }

        //Path to kGrame
        private string path;
        private OnDiskDictionary<string, List<string>> map;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies.false Non stem.</param>
        /// <param name="size">Size of each k-gram term</param>
        public KGram(string path, int size = 3)
        {
            this.size = size;
            this.path = path;
            this.map = new OnDiskDictionary<string, List<string>>(new StringEncoderDecoder(), new StringListEncoderDecoder());
        }

        /// <summary>
        /// Build KGram onto disk
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies</param>
        public KGram buildKGram(HashSet<string> vocabularies)
        {
            Console.WriteLine("Start KGram generating process...");
            Console.WriteLine("Vocbularies' size: " + vocabularies.Count);
            Console.WriteLine("KGram size: " + this.size);

            //Map uses to store data internally
            SortedDictionary<string, List<string>> map = new SortedDictionary<string, List<string>>();

            //Map uses to manp k-gram less than size to k-gram
            SortedDictionary<string, List<string>> miniMap = new SortedDictionary<string, List<string>>();
            //K-gram vocabularies and add them to dictionary

            Console.WriteLine("Building full size KGrams....");
            foreach (string vocab in vocabularies)
            {
                //Split the vocabulary
                List<string> kGrams = this.KGramSplitter("$" + vocab + "$", this.size);

                //Add k-grams to dictionary
                foreach (string kGram in kGrams)
                {
                    if (map.ContainsKey(kGram))
                    {
                        map[kGram].Add(vocab);
                    }
                    else
                    {
                        map.Add(kGram, new List<string> { vocab });
                    }
                }
            }

            //Build lesser k-gram to handle wildcard query lesser than size
            Console.WriteLine("Building lesser size KGrams....");
            foreach (string kGram in map.Keys)
            {
                for (int k = 0; k < this.size; k++)
                {
                    List<string> miniKGrams = this.KGramSplitter(kGram, k);
                    foreach (string miniKGram in miniKGrams)
                    {
                        if (!string.IsNullOrWhiteSpace(miniKGram) && miniKGram != "$")
                        {
                            if (miniMap.ContainsKey(miniKGram))
                            {
                                miniMap[miniKGram].Add(kGram);
                            }
                            else
                            {
                                miniMap.Add(miniKGram, new List<string> { kGram });
                            }
                        }
                    }
                }
            }

            //Print Results
            Console.WriteLine("KGram's size: " + map.Keys.Count);
            Console.WriteLine("Lesser KGram's size: " + miniMap.Keys.Count);

            //WriteKGramToDisk
            Console.WriteLine("Write K-Gram to disk...");
            Console.WriteLine("Path:" + Path.GetFullPath(this.path));


            this.map.Save(map, path, "KGram");
            this.map.Save(miniMap, path, "MiniKGram");
            Console.WriteLine("Complete KGram generating process");
            return this;
        }

        /// <summary>
        /// Get a list of vocabularies for a given kGram
        /// </summary>
        /// <param name="kGram">K-gram to search for</param>
        /// <returns>A list of vocabularies</returns>
        public List<string> getVocabularies(string kGram)
        {
            //If requested k-gram's length is less than this k-gram size, use mini kgram to find the right k-gram 
            if (kGram.Length < this.size)
            {
                HashSet<string> candidates = new HashSet<string>();

                List<string> possibleKGram = this.map.Get(kGram, this.path, "MiniKGram");
                if (possibleKGram == default(List<string>))
                {
                    return new List<string>();
                }

                List<List<string>> KGramLists = new List<List<string>>(this.map.Get(possibleKGram, this.path, "KGram"));
                KGramLists.RemoveAll(item => item == null);

                foreach (List<string> k in KGramLists)
                {
                    foreach (string item in k)
                    {
                        candidates.Add(item);
                    }

                }

                return candidates.ToList();
            }
            else
            {
                List<string> result = this.map.Get(kGram, this.path, "KGram");
                return default(List<string>) == result ? new List<string>() : result;
            }
        }


        /// <summary>
        /// Split kGram
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


    }
}
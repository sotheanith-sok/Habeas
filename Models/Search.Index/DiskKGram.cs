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
    public class DiskKGram
    {
        //size of each k-gram terms.
        public int size { get; }

        private Dictionary<string,List<string>> tempMap;
        private Dictionary<string,List<string>> tempMiniMap;
        //Path to kGrame
        private string path;
        private OnDiskDictionary<string, List<string>> map;
        private OnDiskDictionary<string, List<string>> miniMap;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies.false Non stem.</param>
        /// <param name="size">Size of each k-gram term</param>
        public DiskKGram(string path, int size = 3)
        {
            this.size = size;
            this.path = path;
            this.tempMap= new Dictionary<string, List<string>>();
            this.tempMiniMap = new Dictionary<string, List<string>>();
            this.map = new OnDiskDictionary<string, List<string>>(path, "KGram", new StringEncoderDecoder(), new StringListEncoderDecoder());
            this.miniMap = new OnDiskDictionary<string, List<string>>(path, "MiniKGram", new StringEncoderDecoder(), new StringListEncoderDecoder());
        }

        /// <summary>
        /// Build KGram onto disk
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies</param>
        public DiskKGram buildKGram(HashSet<string> vocabularies)
        {
            Console.WriteLine("Start KGram generating process...");
            Console.WriteLine("Vocbularies' size: " + vocabularies.Count);
            Console.WriteLine("KGram size: " + this.size);

            Console.WriteLine("Building full size KGrams....");
            foreach (string vocab in vocabularies)
            {
                //Split the vocabulary
                List<string> kGrams = this.KGramSplitter("$" + vocab + "$", this.size);

                //Add k-grams to dictionary
                foreach (string kGram in kGrams)
                {
                    if (tempMap.ContainsKey(kGram))
                    {
                        tempMap[kGram].Add(vocab);

                    }
                    else
                    {
                        tempMap.Add(kGram, new List<string> { vocab });
                    }
                }
            }

            //Build lesser k-gram to handle wildcard query lesser than size
            Console.WriteLine("Building lesser size KGrams....");
            foreach (string kGram in tempMap.Keys)
            {
                for (int k = 0; k < this.size; k++)
                {
                    List<string> miniKGrams = this.KGramSplitter(kGram, k);
                    foreach (string miniKGram in miniKGrams)
                    {
                        if (!string.IsNullOrWhiteSpace(miniKGram) && miniKGram != "$")
                        {
                            if (tempMiniMap.ContainsKey(miniKGram))
                            {
                                tempMiniMap[miniKGram].Add(kGram);
                            }
                            else
                            {
                                tempMiniMap.Add(miniKGram, new List<string> { kGram });
                            }
                        }
                    }
                }
            }

            map.Replace(tempMap);
            miniMap.Replace(tempMiniMap);
            //Print Results
            Console.WriteLine("KGram's size: " + map.GetSize());
            Console.WriteLine("Lesser KGram's size: " + miniMap.GetSize());

            //WriteKGramToDisk
            Console.WriteLine("Write K-Gram to disk...");
            // Console.WriteLine("Path:" + Path.GetFullPath(this.path));

            Console.WriteLine("Complete KGram generating process");
            tempMap.Clear();
            tempMiniMap.Clear();
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

                List<string> possibleKGram = this.miniMap.Get(kGram);
                if (possibleKGram == default(List<string>))
                {
                    return new List<string>();
                }

                List<List<string>> KGramLists = new List<List<string>>();

                foreach (string k in possibleKGram)
                {
                    List<string> result = map.Get(k);
                    if (result != default(List<string>))
                    {
                        foreach (string item in result)
                        {
                            candidates.Add(item);
                        }
                    }
                }

                return candidates.ToList();
            }
            else
            {
                List<string> result = this.map.Get(kGram);
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

        public void Clear(){
            map.Clear();
            miniMap.Clear();
        }


    }
}
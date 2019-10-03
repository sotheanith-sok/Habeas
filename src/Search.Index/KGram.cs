using System.Collections.Generic;
using Search.Text;
using System;
namespace Search.Index
{
    /// <summary>
    /// K-gram object uses to store and process k-gram requests
    /// </summary>
    public class KGram
    {
        //Map uses to store data internally
        private Dictionary<string, List<string>> map;

        //size of each k-gram terms.
        public int size { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies.false Non stem.</param>
        /// <param name="size">Size of each k-gram term</param>
        public KGram(HashSet<string> vocabularies, int size = 3)
        {
            this.map = new Dictionary<string, List<string>>();
            this.size = size;
            buildKGram(vocabularies);
        }

        /// <summary>
        /// Construct the k-grams map
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies</param>
        private void buildKGram(HashSet<string> vocabularies)
        {
            foreach (string vocab in vocabularies)
            {
                List<string> kGrams = this.KGramSplitter("$" + vocab + "$");
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
        }

        /// <summary>
        /// Get a list of vocabularies for a given k-gram
        /// </summary>
        /// <param name="kGram">K-gram to search for</param>
        /// <returns>A list of vocabularies</returns>
        public List<string> getVocabularies(string kGram)
        {

            return (this.map.ContainsKey(kGram)) ? this.map[kGram] : new List<string>();
        }


        /// <summary>
        /// Split k-gram
        /// </summary>
        /// <param name="term">term to be split</param>
        /// <returns> list of kgram</returns>
        private List<string> KGramSplitter(string term)
        {
            if (term.Length < this.size)
            {
                return new List<string> { term };
            }
            else
            {
                int i = 0;
                List<string> result = new List<string>();
                while (i + this.size <= term.Length)
                {
                    result.Add(term.Substring(i, this.size));
                    i++;
                }
                return result;
            }

        }

    }
}
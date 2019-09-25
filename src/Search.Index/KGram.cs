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

        private Dictionary<string, List<string>> map;
        private int size;
        private BetterTokenProcessor btp;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vocabularies">List of unique vocabularies</param>
        /// <param name="size">Size of each k-gram term</param>
        public KGram(HashSet<string> vocabularies, int size = 3)
        {
            this.map = new Dictionary<string, List<string>>();
            this.size = size;
            this.btp = new BetterTokenProcessor();
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
                List<string> tokens = this.btp.ProcessToken(vocab, false, false);
                foreach (string token in tokens)
                {
                    List<string> kGrams = this.btp.KGramSplitter(token, this.size);
                    foreach (string kGram in kGrams)
                    {
                        if (this.map.ContainsKey(kGram))
                        {
                            this.map[kGram].Add(token);
                        }
                        else
                        {
                            this.map.Add(kGram, new List<string> { token });
                        }
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

            foreach (KeyValuePair<string, List<string>> kvp in this.map)
            {
                //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            }
            return (this.map.ContainsKey(kGram)) ? this.map[kGram] : new List<string>();
        }

    }
}
using System.Collections.Generic;
using Search.Document;
using Search.Query;
using System.Linq;
using System;

namespace Search.Index
{
    public class SoundExIndex
    {

        public Dictionary<string, List<int>> SoundMap {get;}

        /// <summary>
        /// Constructs SoundExIndex with an empty soundMap
        /// </summary>
        public SoundExIndex(){
            SoundMap = new Dictionary<string, List<int>>();
        }

        /// <summary>
        /// Constructs soundex index(hash map) from the author of documents in the corpus
        /// </summary>
        /// <param name="corpus">the corpus of documents</param>
        public void BuildSoundexIndex(IDocumentCorpus corpus)
        {
      
            foreach (IDocument d in corpus.GetDocuments())
            {
                //Skip document with no author field
                if (d.Author == null) {
                    continue;
                }
                AddDocIdByAuthor(d.Author, d.DocumentId);
     
            ((IDisposable)d).Dispose();
            }
        }

        /// <summary>
        /// Adds docID to the soundexIndex(hashmap) by the sound code of author name as a key
        /// </summary>
        /// <param name="authorName">name to be parsed to sound code and used as key</param>
        /// <param name="docID">document id to be added as value to the hashmap</param>
        public void AddDocIdByAuthor(string authorName, int docID)
        {
            if(authorName == null) { return; }

            //names can consists of more than one name
            string[] terms = authorName.Split(' ');
            //remove the empry string in the string array
            terms = terms.Where(x => !string.IsNullOrEmpty(x)).ToArray();

            foreach (string term in terms)
            {
                //Get the sound code
                string soundCode = ParseToSoundex(term);

                //Add docID to soundMap
                if (SoundMap.ContainsKey(soundCode)) {
                    SoundMap[soundCode].Add(docID);
                } else {
                    SoundMap.Add(soundCode, new List<int> { docID });
                }
            }
        }

        /// <summary>
        /// Parses a string to a soundEx code following SoundEx algorithm
        /// </summary>
        /// <param name="name">a name to be parsed to a soundEx code (name should be one term)</param>
        /// <returns>a soundEx code in string</returns>
        public string ParseToSoundex(string name)
        {
            //SoundEx algorithm
            string soundex = name;

            //1. change string to soundcode
            soundex = Convert2SoundCode(soundex);
            
            //2. remove zero
            soundex = soundex.Replace("0", string.Empty);
            
            //3. remove duplicate of its substring
            soundex = RemoveDuplicateChar(soundex);     // Y24424 -> Y242

            //4. make the length of sound code be 4
            if (soundex.Length < 4) {
                soundex = soundex.PadRight(4, '0');     // Y02 -> Y020
            } else {
                soundex = soundex.Substring(0, 4);      // Y0234 -> Y023
            }

            return soundex;
        }

        ///<summary>///
        ///Converts characters to their proper soundex numerical representation.
        ///</summary>///
        private string Convert2SoundCode(string term)
        {
            term = term.ToUpper();

            //keep the first letter of the string
            string code = term[0].ToString();

            //change all following letters to to proper soundcodes
            foreach(char c in term.Remove(0,1))     
            {
                if ("AEIOUWHY".Contains(c)) {      code += "0"; }
                else if ("BFPV".Contains(c)) {     code += "1"; }
                else if ("CGJKQSXZ".Contains(c)) { code += "2"; }
                else if ("DT".Contains(c)) {       code += "3"; }
                else if ("L".Contains(c)) {        code += "4"; }
                else if ("MN".Contains(c)) {       code += "5"; }
                else {                             code += "6"; }
            }

            return code;
        }

        /// <summary>
        /// Removes duplicate characters from a substring(2) a string
        /// </summary>
        /// <param name="code">a string to be checked</param>
        /// <returns>a string with distinct characters in its substring(2)</returns>
        private string RemoveDuplicateChar(string code)
        {
            if (code.Length < 2) {
                return code;
            }
            string distinct = "";
            string firstTwo = code.Substring(0,2);
            foreach(char c in code.Substring(2)) {
                if(!distinct.Contains(c)) { distinct += c; }
            }
            return firstTwo + distinct;
        }

        /// <summary>
        /// Gets Postings of documents which contain the similar sounding author names.
        /// </summary>
        /// <param name="nameQuery">author name to find documents</param>
        /// <returns>postings with document and empty positions</returns>
        public IList<Posting> GetPostings(string nameQuery){
            IList<Posting> result = new List<Posting>();
            
            //Check author name can contains multiple terms
            string[] terms = nameQuery.Split(' ');
            //remove the empry string in the string array
            terms = terms.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            
            List<IList<Posting>> list = new List<IList<Posting>>();

            foreach(string term in terms) {
                string soundCode = ParseToSoundex(term);
                
                List<int> docIDs;
                try {
                    docIDs = SoundMap[soundCode];
                } catch(KeyNotFoundException){
                    continue;
                }

                IList<Posting> postings = new List<Posting>();
                foreach (int id in docIDs) {
                    postings.Add(new Posting(id, new List<int>()));
                }
                list.Add(postings);
            }

            result = Merge.AndMerge(list);
            return result;
        }

        /// <summary>
        /// Gets all soundex codes of author names from all documents in the corpus
        /// which is stored in soundexIndex
        /// </summary>
        /// <returns>a sorted list of soundex</returns>
        public List<string> GetSoundexVocab(){
            List<string> soundexVocab = SoundMap.Keys.ToList();
            soundexVocab.Sort();
            return soundexVocab;
        }

    }
}
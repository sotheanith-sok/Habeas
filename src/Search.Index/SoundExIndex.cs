using System.Collections.Generic;
using System;
using Search.Document;
using Search.Query;

namespace Search.Index
{
    public class SoundExIndex
    {

        private static Dictionary<string, List<int>> soundMap;

        public SoundExIndex(IDocumentCorpus corpus)
        {
            soundMap = new Dictionary<string, List<int>>();

            BuildSoundExHashMap(corpus);

        }

        /// <summary>
        /// Constructs soundex index(hash map) from the author of documents in the corpus
        /// </summary>
        /// <param name="corpus">the corpus of documents</param>
        public void BuildSoundExHashMap(IDocumentCorpus corpus)
        {
            foreach (IDocument d in corpus.GetDocuments())
            {
                //Skip document with no author field
                if (d.Author == null) {
                    continue;
                }
                
                //names can consists of more than one name
                string[] names = d.Author.Split(' ');
                
                foreach (string name in names)
                {
                    //Get the sound code
                    string soundCode = ParseToSoundCode(name);

                    //Add docID to soundMap
                    if (soundMap.ContainsKey(soundCode)) {
                        soundMap[soundCode].Add(d.DocumentId);
                    } else {
                        soundMap.Add(soundCode, new List<int> { d.DocumentId });
                    }
                }
            }
        }

        /// <summary>
        /// Parse a string to a soundEx code
        /// </summary>
        /// <param name="name">a name to be parsed to a soundEx code</param>
        /// <returns>a soundEx code in string</returns>
        public static string ParseToSoundCode(string name)
        {
            string soundCode;
            soundCode = name.ToUpper();
            soundCode = Change2Numbers(soundCode);
            soundCode = RemoveZeros(soundCode);
            soundCode = RemoveDuplicateChar(soundCode);

            //make the length of sound code be 4
            if (soundCode.Length < 4) {
                soundCode = soundCode.PadRight(4, '0');     // Y02 -> Y020
            } else {
                soundCode = soundCode.Substring(0, 4);      // Y0222 -> Y022
            }

            Console.WriteLine(soundCode);

            return soundCode;
        }

        private static string RemoveZeros(string SoundExCode)
        {
            while (SoundExCode.Contains('0')) {
                for (int i = 0; i < SoundExCode.Length; i++) {
                    if (SoundExCode[i].Equals('0')) {
                        SoundExCode = SoundExCode.Remove(i, 1);
                        break;
                    }
                }
            }
            return SoundExCode;
        }

        ///<summary>///
        ///Converts characters to their proper soundex numerical representation.
        ///</summary>///
        private static string Change2Numbers(string term)
        {
            string code = term[0].ToString();
            for (int i = 1; i < term.Length; i++)
            {
                if ("AEIOUWHY".Contains(term[i])) {
                    code = code + "0";
                }
                else if ("BFPV".Contains(term[i])) {
                    code = code + "1";
                }
                else if ("CGJKQSXZ".Contains(term[i])) {
                    code = code + "2";
                }
                else if ("DT".Contains(term[i])) {
                    code = code + "3";
                }
                else if ("L".Contains(term[i])) {
                    code = code + "4";
                }
                else if ("MN".Contains(term[i])) {
                    code = code + "5";
                }
                else {
                    code = code + "6";
                }
            }

            return code;
        }

        private static string RemoveDuplicateChar(string code)
        {
            string newCode = "";

            if (code.Length < 2) {
                return code;
            }
            else {
                newCode = code.Substring(0, 2);

                for (int i = 2; i < code.Length; i++) {
                    if (!(code[i].Equals(code[i - 1]))) {
                        newCode = newCode + code[i].ToString();
                    }
                }
                return newCode;
            }
        }

        public Dictionary<string, List<int>> getSoundMap()
        {
            return soundMap;
        }

        /// <summary>
        /// Get Posting from by author name.
        /// </summary>
        /// <param name="name">name to retrieve postings by</param>
        /// <returns></returns>
        public IList<Posting> GetPostings(string name){
            IList<Posting> result = new List<Posting>();
            
            //Check author name only contains one
            string[] terms = name.Split(' ');
            List<IList<Posting>> list = new List<IList<Posting>>();

            foreach(string term in terms) {
                string soundCode = ParseToSoundCode(term);
                List<int> docIDs = soundMap[soundCode];
                IList<Posting> postings = new List<Posting>();

                foreach (int id in docIDs) {
                    postings.Add(new Posting(id, new List<int>()));
                }
                list.Add(postings);
            }
            result = Merge.AndMerge(list);

            return result;
        }

    }
}
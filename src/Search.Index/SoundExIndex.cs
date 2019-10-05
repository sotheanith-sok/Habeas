using System.Collections.Generic;
using System;
using Search.Document;

namespace Search.Index
{
    public class SoundExIndex
    {

        private static Dictionary<string, List<int>> soundMap;

        public SoundExIndex(IDocumentCorpus corpus)
        {
            soundMap = new Dictionary<string, List<int>>();

            SoundExHash(corpus);

        }

        public static void SoundExHash(IDocumentCorpus corpus)
        {


            foreach (IDocument d in corpus.GetDocuments())
            {

                if (d.Author != null)
                {
                    string author = d.Author.ToUpper();
                    string soundCode = ParseSoundCode(author);

                    //Add documentID to soundMap
                    if (soundMap.ContainsKey(soundCode)) {
                        soundMap[soundCode].Add(d.DocumentId);
                    }
                    else {
                        soundMap.Add(soundCode, new List<int> { d.DocumentId });
                    }

                }

            }








        }


        public static string ParseSoundCode(string name)
        {
            string soundCode;
            soundCode = Change2Numbers(name.ToUpper());
            soundCode = RemoveZeros(soundCode);
            soundCode = RemoveDuplicateChar(soundCode);

            //make the length of sound code be 4
            if (soundCode.Length < 4) {
                soundCode = soundCode.PadRight(4, '0');     // Y02 -> Y020
            } else {
                soundCode = soundCode.Substring(0, 4);      // Y0222 -> Y022
            }

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
                if ("aeiouwhy".Contains(term[i])) {
                    code = code + "0";
                }
                else if ("bfpv".Contains(term[i])) {
                    code = code + "1";
                }
                else if ("cgjkqsxz".Contains(term[i])) {
                    code = code + "2";
                }
                else if ("dt".Contains(term[i])) {
                    code = code + "3";
                }
                else if ("l".Contains(term[i])) {
                    code = code + "4";
                }
                else if ("mn".Contains(term[i])) {
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
        /// <param name="name">a term of a name to retrieve postings by</param>
        /// <returns></returns>
        public IList<Posting> GetPostings(string name){
            //TODO: is given author name be one here? or 
            //Check author name only 
            throw new NotImplementedException();
        }

    }
}
using System.Collections.Generic;
using System;
using Search.Document;

namespace Search.Index
{
    public class SoundExIndex
    {

        private static Dictionary<string, List<IDocument>> soundMap;

        public SoundExIndex(IDocumentCorpus corpus)
        {
            soundMap = new Dictionary<string, List<IDocument>>();

            SoundExHash(corpus);

        }

        public static void SoundExHash(IDocumentCorpus corpus)
        {


            foreach (IDocument d in corpus.GetDocuments())
            {

                if (d.Author.Length > 0)
                {
                    string SoundExCode;
                    string author = d.Author.ToUpper();

                    SoundExCode = Change2Numbers(author);
                    SoundExCode = RemoveZeros(SoundExCode);
                    SoundExCode = RemoveDuplicateChar(SoundExCode);

                    if (SoundExCode.Length < 4)
                        SoundExCode = SoundExCode.PadRight(4, '0');
                    else
                    {
                        SoundExCode = SoundExCode.Substring(0, 4);
                    }
                    if (soundMap.ContainsKey(SoundExCode))
                    {

                        soundMap[SoundExCode].Add(d);

                    }
                    else
                    {
                        soundMap.Add(SoundExCode, new List<IDocument> { d });
                    }

                }

            }








        }



        public static string RemoveZeros(string SoundExCode)
        {
            while (SoundExCode.Contains('0'))
            {
                for (int i = 0; i < SoundExCode.Length; i++)
                {
                    if (SoundExCode[i].Equals('0'))
                    {

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
        public static string Change2Numbers(string term)
        {
            string code = term[0].ToString();
            for (int i = 1; i < term.Length; i++)
            {
                if ("aeiouwhy".Contains(term[i]))
                {
                    code = code + "0";
                }
                else if ("bfpv".Contains(term[i]))
                {
                    code = code + "1";
                }
                else if ("cgjkqsxz".Contains(term[i]))
                {
                    code = code + "2";
                }
                else if ("dt".Contains(term[i]))
                {
                    code = code + "3";
                }
                else if ("l".Contains(term[i]))
                {
                    code = code + "4";
                }
                else if ("mn".Contains(term[i]))
                {
                    code = code + "5";
                }
                else
                {
                    code = code + "6";
                }
            }

            return code;

        }

        public static string RemoveDuplicateChar(string code)
        {
            string newCode = "";

            if (code.Length < 2)
            {
                return code;
            }
            else
            {
                newCode = code.Substring(0, 2);

                for (int i = 2; i < code.Length; i++)
                {
                    if (!(code[i].Equals(code[i - 1])))
                    {
                        newCode = newCode + code[i].ToString();

                    }
                }

                return newCode;
            }

        }

        public Dictionary<string, List<IDocument>> getSoundMap()
        {
            return soundMap;
        }

    }
}
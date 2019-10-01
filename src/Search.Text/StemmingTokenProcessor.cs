using System.Collections.Generic;
using Porter2Stemmer;
using System;
namespace Search.Text
{
    public class StemmingTokenProcesor : BaseTokenProcessor, ITokenProcessor
    {
        /// <summary>
        /// Process token and stem each terms
        /// </summary>
        /// <param name="token"></param>
        /// <returns>A stem terms</returns>
        public List<string> ProcessToken(string token)
        {
            List<string> result = this.HyphenateWords(token);
            for (int i = 0; i < result.Count; i++)
            {
                result[i] = this.RemoveNonAlphanumeric(result[i]);
                result[i] = this.RemoveApostrophes(result[i]);
                result[i] = this.RemoveQuotationMarks(result[i]);
                result[i] = this.LowercaseWords(result[i]);
            }
            
            for (int i = 0; i < result.Count; i++)
            {
                string[] s = result[i].Split(" ");
                for (int j = 0; j < s.Length; j++)
                {
                    Console.WriteLine(s[j] + "-->" + this.StemWords(s[j]));
                    s[j] = this.StemWords(s[j]);
                }
                result[i] = string.Join(" ", s);
            }
            return result;
        }

        /// <summary>
        /// A token processor uses to generate stem of such token
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing term</returns>
        public string StemWords(string token)
        {
            return new EnglishPorter2Stemmer().Stem(token).Value;
        }
    }
}
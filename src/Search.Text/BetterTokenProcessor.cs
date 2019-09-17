using System;
using System.Collections.Generic;
using Porter2Stemmer;
namespace Search.Text
{
    /// <summary>
    /// An Improved Token Proccessor
    /// </summary>
    public class BetterTokenProcessor
    {
        /// <summary>
        /// Process a token by applying multiple rules
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <returns>List of postprocess tokens</returns>
        public List<string> ProcessToken(string token)
        {
            List<string> tokens = this.HyphenateWords(token);
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i] = this.RemoveNonAlphanumeric(tokens[i]);
                tokens[i] = this.RemoveApostrophes(tokens[i]);
                tokens[i] = this.RemoveQuotationMarks(tokens[i]);
                tokens[i] = this.LowercaseWords(tokens[i]);
                tokens[i] = this.StemWords(tokens[i]);
            }
            return tokens;
        }

        /// <summary>
        /// A token processor uses to remove leading and trailing non-alphanumeric characters
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing token</returns>
        public string RemoveNonAlphanumeric(string token)
        {

            for (int i = 0; i < token.Length; i++)
            {
                if (Char.IsLetter(token[i]) || Char.IsNumber(token[i]))
                {
                    token = token.Substring(i);
                    break;
                }
            }

            for (int i = token.Length - 1; i >= 0; i--)
            {
                if (Char.IsLetter(token[i]) || Char.IsNumber(token[i]))
                {
                    token = token.Substring(0, i + 1);
                    break;
                }

            }

            return token;
        }

        /// <summary>
        /// A token processor uses to remove all apostrophes
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing token</returns>
        public string RemoveApostrophes(string token)
        {
            return token.Replace("'", "");
        }


        /// <summary>
        /// A token processor uses to remove all quotation marks
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing token</returns>
        public string RemoveQuotationMarks(string token)
        {
            return token.Replace("\"", "");
        }


        /// <summary>
        /// A token processor uses to split token by hyphenate and generate a non-hyphenated token
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>A list of postprocessing tokens</returns>
        public List<string> HyphenateWords(string token)
        {
            List<string> result = new List<string>(token.Split("-"));
            if (token.Contains('-'))
            {
                result.Add(token.Replace("-", ""));
            }
            return result;
        }

        /// <summary>
        /// A token processor uses to lower case all characters
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing token</returns>
        public string LowercaseWords(string token)
        {
            return token.ToLower();
        }

        /// <summary>
        /// A token processor uses to generate steam of such token
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing token</returns>
        public string StemWords(string token)
        {
            return new EnglishPorter2Stemmer().Stem(token).Value;
        }

    }

}
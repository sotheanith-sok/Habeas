using System.Collections.Generic;
using System;
namespace Search.Text
{
    public class NormalTokenProcessor : ITokenProcessor
    {
        public List<string> ProcessToken(string token)
        {

            List<string> tokens = this.HyphenateWords(token);
            for (int i = 0; i < tokens.Count; i++)
            {
                tokens[i] = this.RemoveNonAlphanumeric(tokens[i]);
                tokens[i] = this.RemoveApostrophes(tokens[i]);
                tokens[i] = this.RemoveQuotationMarks(tokens[i]);
                tokens[i] = this.LowercaseWords(tokens[i]);
            }

            tokens = tokens.FindAll(c => !string.IsNullOrWhiteSpace(c));
            return tokens;
        }


        /// <summary>
        /// A token processor uses to remove leading and trailing non-alphanumeric characters
        /// </summary>
        /// <param name="token">Preprocessing token</param>
        /// <returns>Postprocessing token</returns>
        public string RemoveNonAlphanumeric(string token)
        {
            List<char> result = new List<char>(token.ToCharArray());
            while (result.Count > 0)
            {
                if (!Char.IsLetter(result[0]) && !Char.IsNumber(result[0]))
                {
                    result.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }

            while (result.Count > 0)
            {
                if (!Char.IsLetter(result[result.Count - 1]) && !Char.IsNumber(result[result.Count - 1]))
                {
                    result.RemoveAt(result.Count - 1);
                }
                else
                {
                    break;
                }
            }

            return new string(result.ToArray());
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
                result.Insert(0, (token.Replace("-", "")));
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
    }
}
using System.Collections.Generic;
using System;
namespace Search.Text
{
    public class NormalTokenProcessor : BaseTokenProcessor, ITokenProcessor
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
            return tokens;
        }
    }
}
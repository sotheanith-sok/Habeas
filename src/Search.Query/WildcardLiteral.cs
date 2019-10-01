using System.Collections.Generic;
using System.Linq;
using Search.Index;
using Search.Text;
namespace Search.Query
{
    public class WildcardLiteral
    {
        private string token;
        private KGram kGram;
        public WildcardLiteral(string token, KGram kGram)
        {
            this.token = token;
            this.kGram = kGram;
        }

        public IList<Posting> GetPostings(IIndex index, ITokenProcessor processor)
        {
            string term = processor.ProcessToken(this.token)[0];
            term = "$" + term + "$";
            string[] literals = term.Split("*");

            List<List<string>> candidatesList = new List<List<string>>();

            foreach (string literal in literals)
            {
                List<string> candidates = null;
                //KGram and AND merge results for a literal                
                List<string> kGramTerms = this.KGramSplitter(literal);
                foreach (string kGramTerm in kGramTerms)
                {
                    List<string> candidate = this.kGram.getVocabularies(kGramTerm);

                    if (candidates == null)
                    {
                        candidates = new List<string>(candidate);
                    }
                    else
                    {
                        candidates = candidates.Intersect(this.kGram.getVocabularies(kGramTerm)).ToList();
                    }
                }


                //Post filtering step

                //$literal*
                if (literal.ElementAt(0) == '$' && literal.ElementAt(literal.Length - 1) != '$')
                {
                    candidates = candidates.Where(s => s.StartsWith(literal.Substring(1))).ToList();
                }

                // *literal$
                else if (literal.ElementAt(0) != '$' && literal.ElementAt(literal.Length - 1) == '$')
                {
                    candidates = candidates.Where(s => s.EndsWith(literal.Substring(0, literal.Length - 2))).ToList();
                }

                // *literal*
                else if (literal.ElementAt(0) != '$' && literal.ElementAt(literal.Length - 1) != '$')
                {
                    candidates = candidates.Where(s => s.Contains(literal)).ToList();
                }

                candidatesList.Add(candidates);
            }


            //Generate the final candidates by merging candidates from all literals
            List<string> finalCandidates = null;
            foreach (List<string> c in candidatesList)
            {
                if (finalCandidates == null)
                {
                    finalCandidates = new List<string>(c);
                }
                else
                {
                    finalCandidates = finalCandidates.Intersect(c).ToList();
                }
            }

            //Get posting
            List<IList<Posting>> finalPostingList = new List<IList<Posting>>();
            foreach (string candidate in finalCandidates)
            {
                finalPostingList.Add(index.GetPostings(candidate));
            }

            return Merge.AndMerge(finalPostingList);
        }

        private List<string> KGramSplitter(string term)
        {
            int i = 0;
            List<string> result = new List<string>();
            while (i + this.kGram.size <= token.Length)
            {
                result.Add(token.Substring(i, this.kGram.size));
                i++;
            }
            return result;

        }
    }
}
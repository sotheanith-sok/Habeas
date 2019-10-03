using System.Collections.Generic;
using System.Linq;
using Search.Index;
using Search.Text;
using System;
using Porter2Stemmer;
namespace Search.Query
{
    /// <summary>
    /// A representation of wildcard literal
    /// </summary>
    public class WildcardLiteral : IQueryComponent
    {
        //Token that user queury
        private string token;

        //KGram for look up
        private KGram kGram;

        private EnglishPorter2Stemmer stemmer;

        /// <summary>
        /// Construct a wildcard literal
        /// </summary>
        /// <param name="token"> Pre-processed token</param>
        /// <param name="kGram"> KGram for lookup</param>
        public WildcardLiteral(string token, KGram kGram)
        {
            this.token = token;
            this.kGram = kGram;
            this.stemmer = new EnglishPorter2Stemmer();
        }

        /// <summary>
        /// Get list of posting
        /// </summary>
        /// <param name="index"> inverted index</param>
        /// <param name="processor">nomal token processor</param>
        /// <returns></returns>
        public IList<Posting> GetPostings(IIndex index, ITokenProcessor processor)
        {
            processor = ((NormalTokenProcessor)processor);
            //Normal proccessing of token and split them into literal by *
            string[] literals = this.token.Split("*");
            for (int i = 0; i < literals.Length; i++)
            {
                if (i == 0)
                {
                    literals[i] = "$" + processor.ProcessToken(literals[i])[0];
                }
                else if (i == literals.Length - 1)
                {
                    literals[i] = processor.ProcessToken(literals[i])[0] + "$";
                }
                else
                {
                    literals[i] = processor.ProcessToken(literals[i])[0];
                }
            }

            literals = literals.Where(x => !string.IsNullOrEmpty(x) && x != "$").ToArray();

            //Gather candidates for each literals
            List<List<string>> candidatesList = new List<List<string>>();

            foreach (string literal in literals)
            {
                List<string> candidates = new List<String>();
                bool didMerge = false;
                //KGram and AND merge results for a literal                
                List<string> kGramTerms = this.KGramSplitter(literal);
                foreach (string kGramTerm in kGramTerms)
                {
                    if (!didMerge)
                    {
                        candidates = candidates.Union(this.kGram.getVocabularies(kGramTerm)).ToList();
                        didMerge = true;
                    }
                    else
                    {
                        candidates = candidates.Intersect(this.kGram.getVocabularies(kGramTerm)).ToList();
                    }
                }

                //Post filtering step
                if (candidates.Count > 0)
                {
                    //$literal*
                    if (literal.ElementAt(0) == '$' && literal.ElementAt(literal.Length - 1) != '$')
                    {
                        candidates = candidates.Where(s => s.StartsWith(literal.Substring(1))).ToList();
                    }

                    // *literal$
                    else if (literal.ElementAt(0) != '$' && literal.ElementAt(literal.Length - 1) == '$')
                    {
                        candidates = candidates.Where(s => s.EndsWith(literal.Substring(0, literal.Length - 1))).ToList();
                    }

                    // *literal*
                    else if (literal.ElementAt(0) != '$' && literal.ElementAt(literal.Length - 1) != '$')
                    {
                        candidates = candidates.Where(s => s.Substring(1, s.Length - 2).Contains(literal)).ToList();
                    }

                    candidatesList.Add(candidates);
                }
                else
                {
                    candidatesList.Add(new List<string>());
                }

            }



            //Generate the final candidates by merging candidates from all literals
            List<string> finalCandidates = new List<string>();
            for (int i = 0; i < candidatesList.Count; i++)
            {
                if (i == 0)
                {
                    finalCandidates = finalCandidates.Union(candidatesList[i]).ToList();
                }
                else
                {
                    finalCandidates = finalCandidates.Intersect(candidatesList[i]).ToList();
                }
            }

            //Get posting for final candidates
            List<IList<Posting>> finalPostingList = new List<IList<Posting>>();
            foreach (string candidate in finalCandidates)
            {

                // finalPostingList.Add(index.GetPostings(stemmer.Stem(candidate).Value));
                finalPostingList.Add(index.GetPostings(candidate));

            }

            return Merge.OrMerge(finalPostingList);
        }

        /// <summary>
        /// Split a term into k gram based on the size in KGram
        /// </summary>
        /// <param name="term">Postprocced but not stem term</param>
        /// <returns>A list of k-gram</returns>
        private List<string> KGramSplitter(string term)
        {
            if (term.Length < this.kGram.size)
            {
                return new List<string> { term };
            }
            else
            {
                int i = 0;
                List<string> result = new List<string>();
                while (i + this.kGram.size <= term.Length)
                {
                    result.Add(term.Substring(i, this.kGram.size));
                    i++;
                }
                return result;
            }
        }
    }
}
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Search.Text;
using Search.Index;
namespace UnitTests
{
    [Collection("FileIORelated")]
    public class KGramTests
    {
        /// <summary>
        /// Test k-gram retrieval
        /// </summary>
        [Fact]
        public void TestKGramRetrieval()
        {
            ITokenProcessor processor = new NormalTokenProcessor();
            List<string> vocabularies = new List<string> { "aPpLe", "apPreciation", "Approachable" };
            for (int i = 0; i < vocabularies.Count; i++)
            {
                vocabularies[i] = processor.ProcessToken(vocabularies[i])[0];
            }
            KGram kGram = new KGram("./").buildKGram(new HashSet<string>(vocabularies));
            Assert.Equal(new List<string> { "apple", "appreciation", "approachable" }, kGram.getVocabularies("$ap"));
        }
    }
}
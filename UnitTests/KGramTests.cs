using Xunit;
using System.Collections.Generic;
using Search.Index;
namespace UnitTests{
    public class KGramTests{

        /// <summary>
        /// Test k-gram retrieval
        /// </summary>
        [Fact]
        public void TestKGramRetrieval(){
            HashSet<string> vocabularies = new HashSet<string>{"aPpLe", "apPreciation","Approachable"};
            KGram kGram = new KGram(vocabularies);
            Assert.Equal(new List<string>{"apple", "appreciation","approachable"},kGram.getVocabularies("$ap"));
        }
    }
}
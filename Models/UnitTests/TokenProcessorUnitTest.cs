using System;
using Xunit;
using Search.Text;
using System.Collections.Generic;

namespace UnitTests
{
    public class TokenProcessorsUnitTest
    {
        private ITokenProcessor normalProcesser, stemmingProcessor;
        public TokenProcessorsUnitTest()
        {
            normalProcesser = new NormalTokenProcessor();
            stemmingProcessor = new StemmingTokenProcesor();
        }


        /// <summary>
        /// Test BetterTokenProccesor's ability to remove leading and trailing all non-alphanumerics
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Theory]
        [InlineData("!@&(*$&Hello.!@#$!@@$#", "Hello")]
        [InlineData("*!@)*)(*$)(*$192.168.1.1(!)@*)*(*($*)(*!@", "192.168.1.1")]
        public void TestRemoveNonAlphanumeric(string token, string expected)
        {
            Assert.Equal(expected, ((NormalTokenProcessor)(this.normalProcesser)).RemoveNonAlphanumeric(token));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to remove all apostrophes 
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Theory]
        [InlineData("'Y'all'll'nt've'd's", "Yallllntveds")]
        public void TestRemoveApostrophes(string token, string expected)
        {
            Assert.Equal(expected, ((NormalTokenProcessor)(this.normalProcesser)).RemoveApostrophes(token));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to remove all quotation marks 
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Theory]
        [InlineData("cklda\"fsafdki\"jfd", "ckldafsafdkijfd")]
        public void TestRemoveQuotationMarks(string token, string expected)
        {
            Assert.Equal(expected, ((NormalTokenProcessor)(this.normalProcesser)).RemoveQuotationMarks(token));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to reformat hyphenated phrases
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Fact]
        public void TestHyphenateWords()
        {
            Assert.Equal(new List<string> { "HewlettPackardComputing", "Hewlett", "Packard", "Computing" }, ((NormalTokenProcessor)(this.normalProcesser)).HyphenateWords("Hewlett-Packard-Computing"));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to lower cases all characters
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Theory]
        [InlineData("QuAntuS TreMoR est FutURUS", "quantus tremor est futurus")]
        public void TestLowercaseWords(string token, string expected)
        {
            Assert.Equal(expected, ((NormalTokenProcessor)(this.normalProcesser)).LowercaseWords(token));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to generate stem
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Theory]
        [InlineData("swimming", "swim")]
        [InlineData("eating", "eat")]
        [InlineData("loving", "love")]

        public void TestStemWords(string token, string expected)
        {
            Assert.Equal(expected, ((StemmingTokenProcesor)(this.stemmingProcessor)).StemWords(token));
        }

        /// <summary>
        /// Test entire token processor with k-gram disable
        /// </summary>
        [Fact]
        public void TestProcessTokenWithoutKGram()
        {
            Assert.Equal(new List<string> { "hello..192.168.1.1.hewlettpackardcomputingjohn legendm","hello","192.168.1.1","hewlett","packard","comput","john legend","m","eat" },
            this.stemmingProcessor.ProcessToken("Hello.-.192.168.1.1.-Hewlett-Packard-Computing-\"John Legend\"-'M'-Eating"));
        }

    }

}

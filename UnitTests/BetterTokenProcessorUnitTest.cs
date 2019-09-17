using System;
using Xunit;
using Search.Text;
using System.Collections.Generic;

namespace CECS_529_Search_Engine_Project
{
    public class BetterTokenProcessorUnitTest
    {
        private BetterTokenProcessor betterTokenProcessor;
        public BetterTokenProcessorUnitTest()
        {
            betterTokenProcessor = new BetterTokenProcessor();
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
            Assert.Equal(expected, this.betterTokenProcessor.RemoveNonAlphanumeric(token));
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
            Assert.Equal(expected, this.betterTokenProcessor.RemoveApostrophes(token));
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
            Assert.Equal(expected, this.betterTokenProcessor.RemoveQuotationMarks(token));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to reformat hyphenated phrases
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Fact]
        public void TestHyphenateWords()
        {
            Assert.Equal(new List<string> { "Hewlett", "Packard", "Computing", "HewlettPackardComputing" }, this.betterTokenProcessor.HyphenateWords("Hewlett-Packard-Computing"));
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
            Assert.Equal(expected, this.betterTokenProcessor.LowercaseWords(token));
        }

        /// <summary>
        /// Test BetterTokenProccesor's ability to generate steam
        /// </summary>
        /// <param name="token">Preprocess token</param>
        /// <param name="expected">Expected postprocess token</param>
        [Theory]
        [InlineData("swimming", "swim")]
        [InlineData("eating", "eat")]
        [InlineData("loving", "love")]

        public void TestStemWords(string token, string expected)
        {
            Assert.Equal(expected, this.betterTokenProcessor.StemWords(token));
        }

        [Fact]
        public void TestProcessToken()
        {
            Assert.Equal(new List<string> { "hello", "192.168.1.1", "hewlett", "packard", "comput", "john legend", "m", "eat", "hello..192.168.1.1.hewlettpackardcomputingjohn legendm" },
            betterTokenProcessor.ProcessToken("Hello.-.192.168.1.1.-Hewlett-Packard-Computing-\"John Legend\"-'M'-Eating"));
        }

    }

}

using Xunit;
using FluentAssertions;
using Search.Query;
using Search.Document;
using Search.Index;
using Search.Text;
using System.Collections.Generic;

namespace UnitTests.QueryTests
{
    public class NearLiteralTests
    {
        private static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../Models/UnitTests/testCorpus3");
        private IIndex index = Indexer.IndexCorpus(corpus);
        private ITokenProcessor processor = new StemmingTokenProcesor();

        [Fact]
        public void GetPostingsTest_NearExist_ReturnsSomePos()
        {
            //Test for exact words
            NearLiteral near1 = new NearLiteral("is", 4, "mystery"); //[is NEAR/4 mystery]
            IList<Posting> result1 = near1.GetPostings(index, processor);
            result1.Should().HaveCount(3);

            //Test for stemmed words
            //[it NEAR/2 snowing] will also search [it NEAR/2 snow] and [it NEAR/2 snows]
            NearLiteral near2 = new NearLiteral("it", 2, "snowing");
            NearLiteral near3 = new NearLiteral("it", 2, "snows");
            IList<Posting> result2 = near2.GetPostings(index, processor);
            IList<Posting> result3 = near3.GetPostings(index, processor);
            result2.Should().HaveSameCount(result3, "because processed \'snowing\' should include result of \'snow\' and \'snows\'");
        }

        [Fact]
        public void GetPostingsTest_NearNotExist_ReturnsEmpty()
        {
            NearLiteral near = new NearLiteral("is", 1, "mystery"); //[is NEAR/1 mystery]
            IList<Posting> result = near.GetPostings(index, processor);

            result.Should().BeEmpty("because there's no document that \'mystery\' appears 1 away from \'is\'");
        }

        //TODO: test for empty string comes in, k=0 or something else, and so on...
    }
}
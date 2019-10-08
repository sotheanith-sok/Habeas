using Xunit;
using FluentAssertions;
using System;
using Search.Query;
using Search.Document;
using Search.Index;
using Search.Text;
using System.Collections.Generic;

namespace UnitTests.Query
{
    public class NearLiteralTests
    {
        private static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus3");
        private IIndex index = new Indexer().IndexCorpus(corpus);
        private ITokenProcessor processor = new StemmingTokenProcesor();

        [Fact]
        public void GetPostingsTest_NearExist_ReturnsSomePos()
        {
            //Test for exact words
            NearLiteral near = new NearLiteral("is", 4, "mystery"); //[is NEAR/4 mystery]
            IList<Posting> result = near.GetPostings(index, processor);
            // IList<Posting> expected = MergeTests.GeneratePostings("...");
            result.Should().HaveCount(3);
            Console.Write(near.ToString() + "\t"); PrintPostingResult(result);

            //Test for stemmed words
            //[it NEAR/2 snowing] will also search [it NEAR/2 snow] and [it NEAR/2 snows]
            NearLiteral near2 = new NearLiteral("it", 2, "snowing");   //[it NEAR/2 snowing]
            result = near2.GetPostings(index, processor);
            // result.Should().HaveCount(4, "because processed \'snowing\' should include result of \'snow\' and \'snows\'");
            Console.Write(near2.ToString() + "\t"); PrintPostingResult(result);
        }

        [Fact]
        public void GetPostingsTest_NearNotExist_ReturnsEmpty()
        {
            NearLiteral near = new NearLiteral("is", 1, "mystery"); //[is NEAR/1 mystery]
            IList<Posting> result = near.GetPostings(index, processor);

            result.Should().BeEmpty("because there's no document that \'mystery\' appears 1 away from \'is\'");
        }


        private void PrintPostingResult(IList<Posting> result)
        {
            foreach (Posting p in result)
            {
                Console.Write(p.ToString() + "  ");
            }
            Console.WriteLine();
        }

    }
}
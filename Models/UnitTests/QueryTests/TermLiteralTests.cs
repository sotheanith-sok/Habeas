using System.Collections.Generic;
using Search.Index;
using Xunit;
using FluentAssertions;
using System;
using Search.Query;
using Search.Document;
using Search.Text;

namespace UnitTests.QueryTests
{
    [Collection("FileIORelated")]
    public class TermLiteralTests
    {
        private static string directory = "../../../Models/UnitTests/testCorpus/testCorpusBasic";
        private static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
        private IIndex index = Indexer.IndexCorpus(corpus);
        private static ITokenProcessor processor = new StemmingTokenProcesor();

        [Fact]
        public void GetPostingsTest_TermExist_ReturnsPostings()
        {
            //1. Test for exact words
            TermLiteral term1 = new TermLiteral("mystery");
            IList<Posting> result = term1.GetPostings(index, processor);
            result.Should().HaveCount(3, "because there are 3 documents that contain the term 'mystery'");

            //2. Test for stemmed words
            TermLiteral term2 = new TermLiteral("snowing");
            TermLiteral term3 = new TermLiteral("snows");
            IList<Posting> result2 = term2.GetPostings(index, processor);
            IList<Posting> result3 = term2.GetPostings(index, processor);
            result2.Should().HaveSameCount(result3, "because 'snowing' should be processed and include result of 'snows', 'snowing' or 'snow'");
        }

        [Fact]
        public void GetPostingsTest_TermNotExist_ReturnsEmpty()
        {
            TermLiteral term = new TermLiteral("zebra");
            IList<Posting> result = term.GetPostings(index, processor);
            result.Should().BeEmpty("because there's no posting for the term 'zebra'");
        }

        [Fact]
        public void GetPostingsTest_TermWithException_ReturnsEmpty()
        {
            TermLiteral term = new TermLiteral("");
            IList<Posting> result = term.GetPostings(index, processor);
            result.Should().BeEmpty("because the term literal has empty string");
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
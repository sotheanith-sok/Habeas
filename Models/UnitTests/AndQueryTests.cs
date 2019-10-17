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
    public class AndQueryTests
    {
        private static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../Models/UnitTests/testCorpus0");
        private IIndex index = Indexer.IndexCorpus(corpus);
        private ITokenProcessor processor = new StemmingTokenProcesor();

        [Fact]
        public void GetPostingsTest_AndQueryExist_ReturnsPostings()
        {
            IList<Posting> result;
            //1. Some postings overlap
            AndQuery andQuery1 = new AndQuery(new List<IQueryComponent>{
                new TermLiteral("snow"),        //0,1,2,4
                new TermLiteral("mystery")      //1,3,4-
            });
            result = andQuery1.GetPostings(index, processor);
            result.Should().HaveCount(2, "because 2 postings contain 'snow' and 'mystery'");
            Console.Write(andQuery1.ToString() + "\t");
            PrintPostingResult(result);

            //2. All postings overlaps
            AndQuery andQuery2 = new AndQuery(new List<IQueryComponent>{
                new TermLiteral("full"),
                new TermLiteral("of")
            });
            result = andQuery2.GetPostings(index, processor);
            result.Should().HaveCount(2, "because all posting from 'full' and 'of' are the same");
            Console.Write(andQuery2.ToString() + "\t");
            PrintPostingResult(result);
        }

        [Fact]
        public void GetPostingsTest_AndQueryNotExist_ReturnsEmpty()
        {
            IList<Posting> result;

            //1. No overlap between two posting lists. distinct from each other
            AndQuery andQuery1 = new AndQuery(new List<IQueryComponent>{
                new TermLiteral("snowing"),
                new TermLiteral("sun")
            });
            result = andQuery1.GetPostings(index, processor);
            result.Should().BeEmpty("because the postings of 'snowing' and 'sun' don't overlap.");

            //2. One component returns empty posting list
            AndQuery andQuery2 = new AndQuery(new List<IQueryComponent>{
                new TermLiteral("full"),
                new TermLiteral("zebra")        //no posting for this
            });
            result = andQuery2.GetPostings(index, processor);
            result.Should().BeEmpty("because one component has no posting");

            //3. All components returns empty posting list
            AndQuery andQuery3 = new AndQuery(new List<IQueryComponent>{
                new TermLiteral("running"),     //no posting for this
                new TermLiteral("zebra")        //no posting for this
            });
            result = andQuery3.GetPostings(index, processor);
            result.Should().BeEmpty("because all components has no posting");
        }

        [Fact]
        public void GetPostingsTest_AndQueryWithEmptyComponents_ReturnsEmpty()
        {
            AndQuery andQuery = new AndQuery(new List<IQueryComponent>());
            IList<Posting> result = andQuery.GetPostings(index, processor);
            result.Should().BeEmpty("because there's no component in AndQuery");
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
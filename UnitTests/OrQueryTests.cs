using System.Collections.Generic;
using Search.Index;
using Xunit;
using FluentAssertions;
using System.Linq;
using System;
using Search.Query;
using Search.Document;
using Search.Text;

namespace UnitTests.Query
{
    public class OrQueryTests
    {
        private static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus3");
        private IIndex index = new Indexer().IndexCorpus(corpus);
        private ITokenProcessor processor = new StemmingTokenProcesor();

        [Fact]
        public void GetPostingsTest_OrQueryExist_ReturnsPostings()
        {
            //Some postings overlap
            OrQuery orQuery1 = new OrQuery(new List<IQueryComponent>{
                new TermLiteral("snow"),
                new TermLiteral("mystery")
            });
            IList<Posting> result = orQuery1.GetPostings(index, processor);
            result.Should().HaveCount(5, "because there are 5 files that contain 'snow' or 'mystery'");
            Console.Write(orQuery1.ToString() + "\t");
            PrintPostingResult(result);

            //All postings overlaps
            OrQuery orQuery2 = new OrQuery(new List<IQueryComponent>{
                new TermLiteral("full"),
                new TermLiteral("of")
            });
            result = orQuery2.GetPostings(index, processor);
            result.Should().HaveCount(2, "because there are 2 files that contain 'full' or 'of'");
            Console.Write(orQuery2.ToString() + "\t");
            PrintPostingResult(result);

            //One component returns empty posting list
            OrQuery orQuery3 = new OrQuery(new List<IQueryComponent>{
                new TermLiteral("full"),
                new TermLiteral("zebra")        //no posting for this
            });
            result = orQuery3.GetPostings(index, processor);
            result.Should().HaveCount(2, "because there are 2 files with 'full' and no file with 'zebra'");
            Console.Write(orQuery3.ToString() + "\t");
            PrintPostingResult(result);
        }

        [Fact]
        public void GetPostingsTest_OrQueryNotExist_ReturnsEmpty()
        {
            OrQuery orQuery = new OrQuery(new List<IQueryComponent>{
                new TermLiteral("running"),
                new TermLiteral("zebra")
            });
            IList<Posting> result = orQuery.GetPostings(index, processor);
            result.Should().BeEmpty("because there is no file with 'running' or 'zebra'");
            Console.Write(orQuery.ToString() + "\t");
            PrintPostingResult(result);
        }

        [Fact]
        public void GetPostingsTest_OrQueryWithEmptyComponents_ReturnsEmpty()
        {
            OrQuery orQuery = new OrQuery(new List<IQueryComponent>());
            IList<Posting> result = orQuery.GetPostings(index, processor);
            result.Should().BeEmpty("because there's no component in OrQuery");
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
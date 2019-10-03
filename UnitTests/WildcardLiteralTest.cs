using Xunit;
using System;
using Search.Document;
using Search.Index;
using Search.Text;
using Search.Query;
using System.Collections.Generic;
using FluentAssertions;
namespace UnitTests
{
    public class WildcardLiteralTest
    {

        [Fact]
        public void TestWildCardLiteral()
        {
            IDocumentCorpus c = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus2");
            IIndex index = PositionalInvertedIndexer.IndexCorpus(c);
            ITokenProcessor processor = new NormalTokenProcessor();
            KGram kGram = PositionalInvertedIndexer.kGram;
            WildcardLiteral wildcard = new WildcardLiteral("*ell*", kGram);
            List<Posting> p = (List<Posting>)(wildcard.GetPostings(index, processor));
            p.Should().BeEquivalentTo(new List<Posting>{new Posting(0, new List<int>{0,1}), new Posting(2, new List<int>{0,2,3})});
        }
    }
}
using Xunit;
using Search.Document;
using Search.Index;
using Search.Text;
using Search.Query;
using System.Collections.Generic;
using FluentAssertions;
using System.Runtime.InteropServices;

namespace UnitTests.Query
{
    public class WildcardLiteralTests
    {
        private static IDocumentCorpus c = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus2");
        private IIndex index = new Indexer().IndexCorpus(c);
        private ITokenProcessor processor = new StemmingTokenProcesor();    //wildcard should use its parent processor anyway
        private KGram kGram = Indexer.kGram;

        [Fact]
        public void WildCardLiteralTest_CircumfixStarts()
        {
            WildcardLiteral wildcard = new WildcardLiteral("*ell*", kGram);        
            IList<Posting> result = wildcard.GetPostings(index, processor);
            
            IList<Posting> expected;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                expected = UnitTest.GeneratePostings("(4,[0,1]), (2,[0,2,3])");
            } else {
                expected = UnitTest.GeneratePostings("(0,[0,1]), (2,[0,2,3])");
            }

            result.Should().BeEquivalentTo(expected);
        }


        [Fact]
        public void WildCardLiteralTest_LeadingStart()
        {
            WildcardLiteral wildcard = new WildcardLiteral("*ello", kGram);        
            IList<Posting> result = wildcard.GetPostings(index, processor);
            
            IList<Posting> expected;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                expected = UnitTest.GeneratePostings("(4,[0,1]), (2,[0,2,3])");
            } else {
                expected = UnitTest.GeneratePostings("(0,[0,1]), (2,[0,2,3])");
            }

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WildCardLiteralTest_TrailingStart()
        {
            WildcardLiteral wildcard = new WildcardLiteral("hell*", kGram);        
            IList<Posting> result = wildcard.GetPostings(index, processor);
            
            IList<Posting> expected;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                expected = UnitTest.GeneratePostings("(4,[0,1]), (2,[0,2,3])");
            } else {
                expected = UnitTest.GeneratePostings("(0,[0,1]), (2,[0,2,3])");
            }

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WildCardLiteralTest_NotExist_ReturnsEmpty()
        {
            //1. Not existing wildcard query
            WildcardLiteral wildcard1 = new WildcardLiteral("zeb*", kGram);        
            IList<Posting> result = wildcard1.GetPostings(index, processor);
            result.Should().BeEmpty("because no posting exist for the wildcard 'zeb*'");

            //2. Exception of putting empty string to wildcard query
            WildcardLiteral wildcard2 = new WildcardLiteral("", kGram);
            result = wildcard2.GetPostings(index, processor);
            result.Should().BeEmpty("because wildcard query got empty string");
        }

    }
}
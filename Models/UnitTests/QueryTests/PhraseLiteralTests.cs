using System.Collections.Generic;
using Search.Index;
using Xunit;
using FluentAssertions;
using Search.Query;
using Search.Document;
using Search.Text;
using System.Runtime.InteropServices;

namespace UnitTests.QueryTests
{
    [Collection("FileIORelated")]
    public class PhraseLiteralTests
    {
        private static string directory = "../../../Models/UnitTests/testCorpus/testCorpusBasic";
        private static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
        private IIndex index = Indexer.IndexCorpus(corpus);
        private static ITokenProcessor processor = new StemmingTokenProcesor();

        [Fact]
        public void GetPostingsTest_PhraseExist_ReturnsPostingsWithFirstPosition()
        {
            IList<Posting> result;
            IList<Posting> expected;

            //1. Test for exact words
            PhraseLiteral phrase1 = new PhraseLiteral("full of mystery");
            result = phrase1.GetPostings(index, processor);
            expected = index.GetPostings("full");
            result.Should().HaveCount(2, "because there are 2 files that contain the phrase 'full of mystery'");
            result.Should().BeEquivalentTo(expected, "because 'full of mystery' should have the same postings with 'full'");

            //2. Test for stemmed words
            PhraseLiteral phrase2 = new PhraseLiteral("hello worlds");
            result = phrase2.GetPostings(index, processor);
            //position of 'hello' in the doc that has "hello" and "world"
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                expected = new List<Posting> { new Posting(4, new List<int> { 1 }) };
            }
            else
            {
                expected = new List<Posting> { new Posting(0, new List<int> { 1 }) };
            }
            result.Should().HaveCount(1, "because 'worlds' should be processed and include result of 'world'");
            result.Should().BeEquivalentTo(expected, "because postings for 'hello worlds' should have the position of 'hello'");
        }

        [Fact]
        public void GetPostingsTest_PhraseNotExist_ReturnsEmpty()
        {
            PhraseLiteral phrase = new PhraseLiteral("snowing in the town");
            IList<Posting> result = phrase.GetPostings(index, processor);
            result.Should().BeEmpty("because there's no document that contains the phrase 'snowing in the town'");
        }

        [Fact]
        public void PhraseLiteralTest_EmptyStringException_ReturnsEmpty()
        {
            PhraseLiteral phrase = new PhraseLiteral("");
            IList<Posting> result = phrase.GetPostings(index, processor);
            result.Should().BeEmpty("because an empty string is passed to the constructor");
        }

    }
}
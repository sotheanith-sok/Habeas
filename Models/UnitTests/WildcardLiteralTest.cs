using Xunit;
using Search.Document;
using Search.Index;
using Search.Text;
using Search.Query;
using System.Collections.Generic;
using FluentAssertions;
using System.Runtime.InteropServices;
using System;
using System.IO;

namespace UnitTests
{
    public class WildcardLiteralTest
    {

        [Fact]
        public void TestWildCardLiteral()
        {
            IDocumentCorpus c = DirectoryCorpus.LoadTextDirectory("../../../Models/UnitTests/testCorpus4");
            Console.WriteLine(Path.GetFullPath("./UnitTests/testCorpus2"));
            IIndex index = Indexer.IndexCorpus(c);
            ITokenProcessor processor = new NormalTokenProcessor();
            KGram kGram = Indexer.kGram;
            WildcardLiteral wildcard = new WildcardLiteral("*ell*", kGram);
            
            IList<Posting> result = (List<Posting>)(wildcard.GetPostings(index, processor));
            
            IList<Posting> expected;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                expected = MergeTests.GeneratePostings("(4,[0,1]), (2,[0,2,3])");
            } else {
                expected = MergeTests.GeneratePostings("(0,[0,1]), (2,[0,2,3])");
            }

            result.Should().BeEquivalentTo(expected);
        }
    }
}
using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;
using FluentAssertions;
using Search;

namespace UnitTests
{
    public class SoundExIndexTest
    {
        [Fact]
        public void testSoundExIndex()
        {

            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");


            SoundExIndex soundIndex = new SoundExIndex(corpus);
            Dictionary<string, List<IDocument>> map = soundIndex.getSoundMap();
            
            var actual = map.ContainsKey("y200");
            
            actual.Should().BeTrue();
            
        }
    }
}

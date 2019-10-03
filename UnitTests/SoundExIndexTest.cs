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
            PositionalInvertedIndex index = PositionalInvertedIndexer.IndexCorpus(corpus);

            SoundExIndex soundIndex = new SoundExIndex(index);
            Dictionary<string, List<string>> map = soundIndex.getSoundMap();
            
    

            var actual = map.ContainsKey("y200");
            

            //Assert
            //actual.Should().Contain(expected);
        }
    }
}

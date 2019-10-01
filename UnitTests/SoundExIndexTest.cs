using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;
using FluentAssertions;

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
            
            Dictionary<String , IList<Posting>> test =  soundIndex.getSoundMap();
            Assert.Equal(test["y200"],index.GetPostings("yashua"));

        }
    }
}

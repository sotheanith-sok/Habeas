using Xunit;
using System.Collections.Generic;
using Search.Document;
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
            var actual = soundIndex.getSoundMap().Keys.Count;
            
            actual.Should().Be(0);
            
        }

        [Theory]
        [InlineData("yashua","Y200")]
        [InlineData("sella","S400")]
        [InlineData("jesse","J200")]
        [InlineData("nith","N300")]
        public void ParseSoundCodeTest(string name, string expected)
        {
            var actual = SoundExIndex.ParseToSoundCode(name);
            actual.Should().Be(expected);
        }
    }
}

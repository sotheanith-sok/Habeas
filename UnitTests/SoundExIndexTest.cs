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
    public class SoundExIndexTests
    {
        [Fact]
        public void testSoundExIndex()
        {
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");

            SoundExIndex soundIndex = new SoundExIndex(corpus);
            //var actual = soundIndex.getSoundMap().Keys.Count;
            
            //actual.Should().Be(0);
            
        }

        [Theory]
        [InlineData("ovando","O153")]
        [InlineData("bae","B000")]
        [InlineData("blacklock","B424")]
        [InlineData("sok","S200")]
        public void ParseToSoundCodeTest(string name, string expected)
        {
            var actual = new SoundExIndex().ParseToSoundCode(name);
            actual.Should().Be(expected);
        }

        [Fact]
        public void GetPostingsTest_OneName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");
            SoundExIndex authorIndex = new SoundExIndex();
            authorIndex.BuildSoundExHashMap(corpus);
            //Act
            var actual = authorIndex.GetPostings("yash");
            //Assert
            actual.Should().HaveCount(3);
        }

        [Fact]
        public void GetPostingsTest_MultipleNames()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");
            SoundExIndex authorIndex = new SoundExIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("yashua ovando");
            //Assert
            actual.Should().HaveCount(1);
        }

        [Fact]
        public void GetPostingsTest_SimilarSoundingName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");
            SoundExIndex authorIndex = new SoundExIndex(corpus);
            //Act
            var result1 = authorIndex.GetPostings("bloclic");
            var result2 = authorIndex.GetPostings("blacklock");
            //Assert
            result1.Should().BeEquivalentTo(result2);
        }

        [Fact]
        public void GetPostingsTest_NotExistingName_ReturnsEmpty()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");
            SoundExIndex authorIndex = new SoundExIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("hella");
            //Assert
            actual.Should().BeEmpty();
        }

    }
}

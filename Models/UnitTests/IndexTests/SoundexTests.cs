using Xunit;
using Search.Document;
using Search.Index;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests.IndexTests
{
    [Collection("FileIORelated")]
    public class SoundexTests
    {
        string directory = "../../../Models/UnitTests/testCorpus/testCorpusWithAuthor";

        [Fact]
        public void SoundexIndexTest()
        {
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundEx soundIndex = new SoundEx("./");
            soundIndex.BuildSoundexIndex(corpus);
            soundIndex.GetCount().Should().Be(5);
        }

        [Theory]
        [InlineData("ovando", "O153")]
        [InlineData("bae", "B000")]
        [InlineData("blacklock", "B424")]
        [InlineData("sok", "S200")]
        public void ParseToSoundexTest(string name, string expected)
        {
            var actual = new SoundEx("./").ParseToSoundex(name);
            actual.Should().Be(expected);
        }

        [Fact]
        public void AddDocIDByAuthorTest()
        {
            //Arrange
            SoundEx authorIndex = new SoundEx("./");

            SortedDictionary<string, List<int>> dictionary = new SortedDictionary<string, List<int>>();

            //Act
            authorIndex.AddDocIdByAuthor("sella", 1, dictionary);
            authorIndex.AddDocIdByAuthor("selly", 2, dictionary);
            authorIndex.AddDocIdByAuthor("yashua", 3, dictionary);
            authorIndex.AddDocIdByAuthor("yoshi", 4, dictionary);
            authorIndex.AddDocIdByAuthor("yesh", 5, dictionary);

            authorIndex.BuildSoundexIndex(dictionary);
            //Assert
            authorIndex.GetSoundexVocab().Should().HaveCount(2);
            authorIndex.Get("S440").Should().HaveCount(2);
            authorIndex.Get("Y200").Should().HaveCount(3);
        }

        [Fact]
        public void AddDocIDByAuthorTest_InputExceptionWithSpace()
        {
            //Arrange
            SoundEx authorIndex = new SoundEx("./");

            SortedDictionary<string, List<int>> dictionary = new SortedDictionary<string, List<int>>();
            //Act
            authorIndex.AddDocIdByAuthor(" sella", 1, dictionary);
            authorIndex.AddDocIdByAuthor("sella  ", 2, dictionary);
            authorIndex.AddDocIdByAuthor("yashua  ovando", 2, dictionary);
            authorIndex.AddDocIdByAuthor(" yashua     ovando     ", 3, dictionary);
            authorIndex.BuildSoundexIndex(dictionary);
            //Assert
            authorIndex.GetSoundexVocab().Should().HaveCount(3);
        }

        [Fact]
        public void GetPostingsTest_OneName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundEx authorIndex = new SoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("yash");
            //Assert
            actual.Should().HaveCount(3);
        }

        [Fact]
        public void GetPostingsTest_MultipleNames()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundEx authorIndex = new SoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("yashua ovando");
            //Assert
            actual.Should().HaveCount(2);
        }

        [Fact]
        public void GetPostingsTest_SimilarSoundingName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundEx authorIndex = new SoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
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
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundEx authorIndex = new SoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("hella");
            //Assert
            actual.Should().BeEmpty();
        }

    }
}

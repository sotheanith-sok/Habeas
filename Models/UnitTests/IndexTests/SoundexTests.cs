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
            DiskSoundEx soundIndex = new DiskSoundEx("./");
            soundIndex.BuildSoundexIndex(corpus);
            soundIndex.GetCount().Should().Be(5);
            soundIndex.Clear();
        }

        [Theory]
        [InlineData("ovando", "O153")]
        [InlineData("bae", "B000")]
        [InlineData("blacklock", "B424")]
        [InlineData("sok", "S200")]
        public void ParseToSoundexTest(string name, string expected)
        {
            DiskSoundEx soundEx = new DiskSoundEx("./");
            var actual = soundEx.ParseToSoundex(name);
            actual.Should().Be(expected);
            soundEx.Clear();
        }

        [Fact]
        public void AddDocIDByAuthorTest()
        {
            //Arrange
            DiskSoundEx authorIndex = new DiskSoundEx("./");
            //Act
            authorIndex.AddDocIdByAuthor("sella", 1);
            authorIndex.AddDocIdByAuthor("selly", 2);
            authorIndex.AddDocIdByAuthor("yashua", 3);
            authorIndex.AddDocIdByAuthor("yoshi", 4);
            authorIndex.AddDocIdByAuthor("yesh", 5);
            authorIndex.Save();

            //Assert
            authorIndex.GetSoundexVocab().Should().HaveCount(2);
            authorIndex.Get("S440").Should().HaveCount(2);
            authorIndex.Get("Y200").Should().HaveCount(3);

            authorIndex.Clear();
        }

        [Fact]
        public void AddDocIDByAuthorTest_InputExceptionWithSpace()
        {
            //Arrange
            DiskSoundEx authorIndex = new DiskSoundEx("./");

            //Act
            authorIndex.AddDocIdByAuthor(" sella", 1);
            authorIndex.AddDocIdByAuthor("sella  ", 2);
            authorIndex.AddDocIdByAuthor("yashua  ovando", 2);
            authorIndex.AddDocIdByAuthor(" yashua     ovando     ", 3);
            authorIndex.Save();
            //Assert
            authorIndex.GetSoundexVocab().Should().HaveCount(3);
            authorIndex.Clear();
        }

        [Fact]
        public void GetPostingsTest_OneName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            DiskSoundEx authorIndex = new DiskSoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("yash");
            //Assert
            actual.Should().HaveCount(3);
            authorIndex.Clear();
        }

        [Fact]
        public void GetPostingsTest_MultipleNames()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            DiskSoundEx authorIndex = new DiskSoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("yashua ovando");
            //Assert
            actual.Should().HaveCount(2);
            authorIndex.Clear();
        }

        [Fact]
        public void GetPostingsTest_SimilarSoundingName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            DiskSoundEx authorIndex = new DiskSoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var result1 = authorIndex.GetPostings("bloclic");
            var result2 = authorIndex.GetPostings("blacklock");
            //Assert
            result1.Should().BeEquivalentTo(result2);
            authorIndex.Clear();
        }

        [Fact]
        public void GetPostingsTest_NotExistingName_ReturnsEmpty()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            DiskSoundEx authorIndex = new DiskSoundEx("./");
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("hella");
            //Assert
            actual.Should().BeEmpty();
            authorIndex.Clear();
        }

    }
}

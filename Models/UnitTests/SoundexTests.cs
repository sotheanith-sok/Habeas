using Xunit;
using Search.Document;
using Search.Index;
using FluentAssertions;

namespace UnitTests
{
    [Collection("FileIORelated")]
    public class SoundexTests
    {
        string directory = "../../../Models/UnitTests/testCorpus";

        [Fact]
        public void SoundexIndexTest()
        {
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundExIndex soundIndex = new SoundExIndex();
            soundIndex.BuildSoundexIndex(corpus);
            var actual = soundIndex.SoundMap;
            actual.Keys.Count.Should().Be(5);
        }

        [Theory]
        [InlineData("ovando", "O153")]
        [InlineData("bae", "B000")]
        [InlineData("blacklock", "B424")]
        [InlineData("sok", "S200")]
        public void ParseToSoundexTest(string name, string expected)
        {
            var actual = new SoundExIndex().ParseToSoundex(name);
            actual.Should().Be(expected);
        }

        [Fact]
        public void AddDocIDByAuthorTest()
        {
            //Arrange
            SoundExIndex authorIndex = new SoundExIndex();
            //Act
            authorIndex.AddDocIdByAuthor("sella", 1);
            authorIndex.AddDocIdByAuthor("selly", 2);
            authorIndex.AddDocIdByAuthor("yashua", 3);
            authorIndex.AddDocIdByAuthor("yoshi", 4);
            authorIndex.AddDocIdByAuthor("yesh", 5);
            //Assert
            authorIndex.GetSoundexVocab().Should().HaveCount(2);
            authorIndex.SoundMap["S440"].Should().HaveCount(2);
            authorIndex.SoundMap["Y200"].Should().HaveCount(3);
        }

        [Fact]
        public void AddDocIDByAuthorTest_InputExceptionWithSpace()
        {
            //Arrange
            SoundExIndex authorIndex = new SoundExIndex();
            //Act
            authorIndex.AddDocIdByAuthor(" sella", 1);
            authorIndex.AddDocIdByAuthor("sella  ", 2);
            authorIndex.AddDocIdByAuthor("yashua  ovando", 2);
            authorIndex.AddDocIdByAuthor(" yashua     ovando     ", 3);
            //Assert
            authorIndex.GetSoundexVocab().Should().HaveCount(3);
        }

        [Fact]
        public void GetPostingsTest_OneName()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(directory);
            SoundExIndex authorIndex = new SoundExIndex();
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
            SoundExIndex authorIndex = new SoundExIndex();
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
            SoundExIndex authorIndex = new SoundExIndex();
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
            SoundExIndex authorIndex = new SoundExIndex();
            authorIndex.BuildSoundexIndex(corpus);
            //Act
            var actual = authorIndex.GetPostings("hella");
            //Assert
            actual.Should().BeEmpty();
        }

    }
}

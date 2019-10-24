using System.IO;
using Xunit;
using FluentAssertions;
using Search.Index;
using Search.Document;

namespace UnitTests.OnDiskIndexTests
{
    [Collection("FileIORelated")]
    public class DiskIndexWriterTests
    {
        string corpusDir = "../../../Models/UnitTests/testCorpus/testCorpusBasic";
        string dirPath = "../../../Models/UnitTests/testCorpus/testCorpusBasic/index/";

        [Fact]
        public void BinaryWriterTest()
        {
            //Just testing the BinaryWriter
            Directory.CreateDirectory(dirPath);
            string filePath = dirPath + "test.bin";

            File.Create(filePath).Dispose();
            BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
            writer.Write(16);   // 10(hex)
            writer.Write(40);   // 28(hex)

            long length = writer.BaseStream.Length;
            length.Should().Be( 2 * 4 );  // two 4-byte integers
            
            writer.Dispose();
        }

        // [Fact]
        // public void WritePostingTest()
        // {
        //     Directory.CreateDirectory(dirPath);
        //     //Arrange
        //     string filePath = dirPath + "postings.bin";
        //     File.Create(filePath).Dispose();
        //     BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
        //     DiskIndexWriter indexWriter = new DiskIndexWriter();
        //     IList<Posting> postings;
        //     long startByte;
        //     long actualLength;

        //     //Act
        //     postings = UnitTest.GeneratePostings("(10,[16,17]), (32,[20,26])");
        //     //with gaps (10 [16, 1]), (22 [20, 6])  --> 0A 10 01 16 14 06 (hex)
        //     startByte = indexWriter.WritePostings(postings, writer);
        //     actualLength = writer.BaseStream.Length;
        //     //Assert
        //     actualLength.Should().Be( 6 * 4 );  // six 4-byte integers
        //     startByte.Should().Be(0);

        //     //Act2
        //     postings = UnitTest.GeneratePostings("(7,[160,161])");
        //     //with gaps (7 [160, 1])  --> 07 A0 01 (hex)
        //     startByte = indexWriter.WritePostings(postings, writer);
        //     actualLength = writer.BaseStream.Length;
        //     //Assert2
        //     actualLength.Should().Be( 9 * 4 );  // nine 4-byte integers so far
        //     startByte.Should().Be(24);  // where the first docID of this postings starts

        //     writer.Dispose();
        // }

        // [Fact]
        // public void WriteVocabTableTest()
        // {
        //     Directory.CreateDirectory(dirPath);
        //     //Arrange
        //     string filePath = dirPath + "vocabTable.bin";
        //     File.Create(filePath).Dispose();
        //     BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
        //     DiskIndexWriter indexWriter = new DiskIndexWriter();

        //     //Act
        //     indexWriter.WriteVocabTable(4, 160, writer);    //04 A0
        //     indexWriter.WriteVocabTable(8, 169, writer);    //08 A9
        //     indexWriter.WriteVocabTable(12, 176, writer);   //0C B0
            
        //     //Assert
        //     long length = writer.BaseStream.Length;
        //     length.Should().Be(6 * 8);  // six 8-byte integers
            
        //     writer.Dispose();
        // }

        [Fact]
        public void WriteIndexTest()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(corpusDir);
            PositionalInvertedIndex index = Indexer.IndexCorpus(corpus);
            
            //Act
            DiskIndexWriter indexWriter = new DiskIndexWriter();
            indexWriter.WriteIndex(index, corpusDir+"/index/");

            //Assert
            File.Exists(corpusDir+"/index/vocab.bin").Should().BeTrue();
            File.Exists(corpusDir+"/index/postings.bin").Should().BeTrue();
            File.Exists(corpusDir+"/index/vocabTable.bin").Should().BeTrue();
            // int expectedVocabLength = 0; //??
            // File.ReadAllBytes(corpusDir+"/index/vocab.bin").Length.Should().Be(expectedVocabLength);
            int expectedPostingsLength = (13 + 34 + 75) * 4;   // (# of documentFrequencies + # of docIDs + # of termFrequencies + # of positions) * byteSize
            File.ReadAllBytes(corpusDir+"/index/postings.bin").Length.Should().Be(expectedPostingsLength);
            int expectedVocabTableLength = 13 * 2 * 8;
            File.ReadAllBytes(corpusDir+"/index/vocabTable.bin").Length.Should().Be(expectedVocabTableLength);

        }

    }
}
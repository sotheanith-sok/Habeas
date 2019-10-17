using System.IO;
using Xunit;
using FluentAssertions;
using Search.Index;
using System.Collections.Generic;

namespace UnitTests
{
    public class DiskIndexWriterTests
    {
        string directory = "../../../Models/UnitTests/testCorpus3/index/";

        [Fact]
        public void BinaryWriterTest()
        {
            //Just testing the BinaryWriter
            
            string filePath = directory + "vocab.bin";
            // BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create));
            // File.Open(filePath, FileMode.Create).Dispose();
            File.Create(filePath).Dispose();
            BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
            
            // writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
            writer.Write(16);
            writer.Write(40);

            long actualLength = writer.BaseStream.Length;
            long byteSize = 4;
            long expectedLength = 2 * byteSize;

            actualLength.Should().Be(expectedLength);
        }

        [Fact]
        public void WritePostingTest(){
            //Arrange
            long byteSize = 4;
            string filePath = directory + "postings.bin";
            File.Create(filePath).Dispose();
            BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
            DiskIndexWriter indexWriter = new DiskIndexWriter();
            IList<Posting> postings;
            long startByte;
            long actualLength;
            long expectedLength;

            //Act
            postings = UnitTest.GeneratePostings("(10,[16,17]), (32,[20,26])"); // 0A, 10, 11, 20, 14, 1A (hex)
            startByte = indexWriter.WritePostings(postings, writer);
            actualLength = writer.BaseStream.Length;
            expectedLength = 6 * byteSize;
            //Assert
            actualLength.Should().Be( expectedLength );
            startByte.Should().Be(0);

            //Act2
            postings = UnitTest.GeneratePostings("(7,[160,161])");    // 07, A0, A1 (hex)
            startByte = indexWriter.WritePostings(postings, writer);
            actualLength = writer.BaseStream.Length;
            expectedLength = expectedLength + 3 * byteSize;
            //Assert2
            actualLength.Should().Be( expectedLength );
            startByte.Should().Be(24);
        }
    }
}
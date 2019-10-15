using System.IO;
using Xunit;
using FluentAssertions;

namespace UnitTests
{
    public class DiskIndexWriterTests
    {
        string directory = "../../../Models/UnitTests/testCorpus3";

        [Fact]
        public void BinaryWriterTest()
        {
            //Just testing the BinaryWriter
            
            string filePath = directory + "/index/vocab.bin";
            BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.OpenOrCreate));
            // writer = new BinaryWriter(File.Open(filePath, FileMode.Append));
            writer.Write(10);

            long actualLength = writer.BaseStream.Length;
            long expectedLength = 1;

            actualLength.Should().Be(expectedLength);
        }
    }
}
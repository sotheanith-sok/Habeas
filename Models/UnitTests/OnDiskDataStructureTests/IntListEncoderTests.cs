using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Search.OnDiskDataStructure;

namespace UnitTests.OnDiskDataStructureTests
{
    public class IntListEncoderTests
    {

        [Fact]
        public void IntListEncodingTest()
        {
            //Arrange
            List<int> list = new List<int>(){128, 0, 255};
            var encoder = new IntListEncoderDecoder();
            //Act
            var actual = encoder.Encoding(list);
            //Assert
            var expected = new byte[]{0x01, 0x80, 0x80, 0x01, 0xFF}; //128, 0, 255
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void IntListDecodingTest()
        {
            //Arrange
            byte[] encoded = new byte[]{0x01, 0x80, 0x80, 0x01, 0xFF}; //128, 0, 255
            var encoder = new IntListEncoderDecoder();
            //Act
            var actual = encoder.Decoding(encoded);
            //Assert
            var expected = new List<int>(){128, 0, 255};
            actual.Should().BeEquivalentTo(expected);
        }
        

        [Fact]
        public void IntEncodingTest()
        {
            var encoder = new IntEncoderDecoder();
            var actual = encoder.Encoding(128);
            actual.Should().BeEquivalentTo(new byte[]{0x01, 0x80});
        }

        [Fact]
        public void IntDecodingTest()
        {
            var encoder = new IntEncoderDecoder();
            var actual = encoder.Decoding(new byte[]{0x01, 0x80});
            actual.Should().Be(128);
        }
    }
}
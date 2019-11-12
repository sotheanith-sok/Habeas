using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using Search.OnDiskDataStructure;
using System;

namespace UnitTests.OnDiskDataStructureTests
{
    public class VariableBytesTests
    {
        
        [Theory]
        [InlineData(0, new byte[]{0b10000000})]
        [InlineData(129, new byte[]{0b00000001, 0b10000001})]
        [InlineData(2147483647, new byte[]{0b00000111, 0b01111111, 0b01111111, 0b01111111, 0b11111111})] //2^31-1
        [InlineData(16384, new byte[]{0b00000001, 0b00000000, 0b10000000})] //2^14
        public void EncodeTest_OneInt(int num, byte[] expected)
        {
            var actual = VariableBytes.Encode(num);
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void EncodeTest_IntList()
        {
            var actual = VariableBytes.Encode(new List<int>(){128, 0, 255});
            var expected = new byte[]{0x01, 0x80, 0x80, 0x01, 0xFF}; //128, 0, 255
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void DecodeTest()
        {
            var actual = VariableBytes.Decode(new byte[]{0b00000001, 0b10000000});
            var expected = 128;
            actual.Should().Be(expected);
        }

        [Fact]
        public void EncodedByteStreamTest()
        {
            //Arrange
            byte[] value = new byte[]{0b00000001, 0b10000000, 0b10000000, 0b00000001, 0b10000001, 0b10001000}; //128, 0, 129, 8
            var encodedBytes = new VariableBytes.EncodedByteStream(value);

            //Act&Assert
            
            //Test Extract()
            var actual = encodedBytes.Extract();
            var expected = new byte[]{0b00000001, 0b10000000};

            actual.Should().BeEquivalentTo(expected);
            encodedBytes.Pos.Should().Be(2);

            //Test Skip()
            encodedBytes.Skip();
            encodedBytes.Pos.Should().Be(3);

            //Test ReadInt()
            var actualNum = encodedBytes.ReadDecodedInt();
            actualNum.Should().Be(129);
            encodedBytes.Pos.Should().Be(5);
            
        }
        
    }
}
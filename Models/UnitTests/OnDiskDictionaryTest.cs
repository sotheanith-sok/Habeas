using Xunit;
using System.Collections.Generic;
using Search.OnDiskDataStructure;
namespace UnitTests
{
    [Collection("FileIORelated")]

    /// <summary>
    /// Test on-disk dictionary
    /// </summary>
    public class OnDiskDictionaryTest
    {
        /// <summary>
        /// Test 1
        /// </summary>
        [Fact]
        public void TestWriteToDisk()
        {
            Dictionary<string, string> testValues = new Dictionary<string, string>();
            testValues.Add("A", "Apple");
            testValues.Add("B", "Banana");
            testValues.Add("C", "Cat");
            testValues.Add("D", "Dog");
            testValues.Add("E", "Eye");
            IEncoderDecoder<string> encoderDecoder = new StringEncoderDecoder();
            new OnDiskDictionary<string, string>().Save(encoderDecoder, encoderDecoder, testValues, "./", "TestAlphabet");
            Assert.Equal("Apple", new OnDiskDictionary<string, string>().Get(encoderDecoder, encoderDecoder, "A", "./", "TestAlphabet"));
            Assert.Equal("Eye", new OnDiskDictionary<string, string>().Get(encoderDecoder, encoderDecoder, "E", "./", "TestAlphabet"));
            Assert.Equal("Banana", new OnDiskDictionary<string, string>().Get(encoderDecoder, encoderDecoder, "B", "./", "TestAlphabet"));
            Assert.Equal("Dog", new OnDiskDictionary<string, string>().Get(encoderDecoder, encoderDecoder, "D", "./", "TestAlphabet"));
            Assert.Equal("Cat", new OnDiskDictionary<string, string>().Get(encoderDecoder, encoderDecoder, "C", "./", "TestAlphabet"));
        }
    }
}

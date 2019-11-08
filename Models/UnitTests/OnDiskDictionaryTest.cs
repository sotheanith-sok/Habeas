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
            OnDiskDictionary<string, string> dic = new OnDiskDictionary<string, string>(new StringEncoderDecoder(), new StringEncoderDecoder());
            dic.Save(testValues, "./", "TestAlphabet");
            Assert.Equal("Apple", dic.Get("A", "./", "TestAlphabet"));
            Assert.Equal("Eye", dic.Get("E", "./", "TestAlphabet"));
            Assert.Equal("Banana", dic.Get("B", "./", "TestAlphabet"));
            Assert.Equal("Dog", dic.Get("D", "./", "TestAlphabet"));
            Assert.Equal("Cat", dic.Get("C", "./", "TestAlphabet"));
        }
    }
}

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
            OnDiskDictionary<string, string> dic = new OnDiskDictionary<string, string>("./", "TestAlphabet", new StringEncoderDecoder(), new StringEncoderDecoder());

            dic.Add("A", "Apple");
            dic.Add("B", "Banana");
            dic.Add("C", "Cat");
            dic.Add("D", "Dog");
            dic.Add("E", "Eye");
            Assert.Equal("Apple", dic.Get("A"));
            Assert.Equal("Eye", dic.Get("E"));
            Assert.Equal("Banana", dic.Get("B"));
            Assert.Equal("Dog", dic.Get("D"));
            Assert.Equal("Cat", dic.Get("C"));
            dic.Clear();
        }
    }
}

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
            SortedDictionary<string, string> testValues = new SortedDictionary<string, string>();
            testValues.Add("A", "Apple");
            testValues.Add("B", "Banana");
            testValues.Add("C", "Cat");
            testValues.Add("D", "Dog");
            testValues.Add("E", "Eye");
            OnDiskDictionary<string, string> dic = new OnDiskDictionary<string, string>("./", "TestAlphabet", new StringEncoderDecoder(), new StringEncoderDecoder());
            dic.Save(testValues);
            Assert.Equal("Apple", dic.Get("A"));
            Assert.Equal("Eye", dic.Get("E"));
            Assert.Equal("Banana", dic.Get("B"));
            Assert.Equal("Dog", dic.Get("D"));
            Assert.Equal("Cat", dic.Get("C"));
        }
    }
}

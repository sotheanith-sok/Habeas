using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Search.Text;
using Search.Query;
using FluentAssertions;
using Search.Index;
using Xunit;
namespace UnitTests
{
    public class RankedRetrievalParserTest
    {
        [Fact]
        public void TestParsing()
        {

            RankedRetrievalParser test = new RankedRetrievalParser();
            List<string> actual = test.ParseQuery("test");
            List<string> actual1 = test.ParseQuery("national park");

            actual.Should().BeEquivalentTo("test");


            actual1.Should().BeEquivalentTo("nation","park");

        }
        
    }
}
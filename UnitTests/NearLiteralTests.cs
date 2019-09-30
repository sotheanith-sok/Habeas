using System.Collections.Generic;
using Search.Index;
using Xunit;
using FluentAssertions;
using System.Linq;
using System;
using Search.Query;

namespace UnitTests
{
    public class NearLiteralTests
    {
        // [Fact]
        public void GetPostingsTest_NearExist_ReturnsSomePos()
        {
            //Build index
            //Create NearLiteral object NearLinteral(first, k, second)
            //Call GetPostings()
            //Compare with ...
        }
        // [Fact]
        public void GetPostingsTest_NearNotExist_ReturnsEmpty()
        {
            //...
        }

    }
}
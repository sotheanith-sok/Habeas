/// <param name="term">a processed string</param>
using System.Collections.Generic;
using System.Linq;
using Search.Query;
using System;
using Search.OnDiskDataStructure;
using System.IO;
using FluentAssertions;
using Search.Index;
using Xunit;
using System.Globalization;
namespace UnitTests.DiskIndexTest
{
    public class SpecialIndexTests
    {
        private static string dirPath = "../../../Models/UnitTests/testCorpus/testCorpusBasic";

        [Fact]
        public void GetRankedDocumentsTest()
        {
            SpecialIndex testIndex = new SpecialIndex(dirPath);
            string[] terms = {"hello"};
            testIndex.GetRankedDocuments(terms);
        }
    }

}
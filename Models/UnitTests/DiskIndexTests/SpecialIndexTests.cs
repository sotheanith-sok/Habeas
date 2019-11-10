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
using Search.Document;
using System.Globalization;
namespace UnitTests.DiskIndexTest
{
    public class SpecialIndexTests
    {
        private static string corpusDir = "../../../Models/UnitTests/testCorpus/testCorpusBasic";

        [Fact]
        public void GetRankedDocumentsTest()
        {
            string pathToIndex = Path.Join(corpusDir, "/index/");
            Indexer.path = pathToIndex;
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(corpusDir);


            Directory.CreateDirectory(Path.Join(corpusDir, "/index/"));
            IIndex index = Indexer.IndexCorpus(corpus);

            List<string> terms= new List<string>();
            terms.Add("hello");
            index.GetRankedDocuments(terms);
        }
    }

}
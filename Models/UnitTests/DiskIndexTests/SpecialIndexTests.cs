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
    [Collection("FileIORelated")]
    public class SpecialIndexTests
    {
        private static string corpusDir = "../../../Models/UnitTests/testCorpus/testCorpusBasic";

        [Fact]
        public void GetRankedDocumentsTest()
        {
            //Path to where all the bin file will be write to
            string pathToIndex = Path.Join(corpusDir, "/index/");

            //Let Indexer know where should it writes all bin files
            Indexer.path = pathToIndex;

            //Read corpus
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory(corpusDir);

            //Create directory  to bins folders if it doesn't exist
            Directory.CreateDirectory(Path.Join(corpusDir, "/index/"));

            //Initialize the index. 
            IIndex index = Indexer.IndexCorpus(corpus);

            //The rest of your code...
            List<string> terms= new List<string>();
            terms.Add("hello");
            index.GetRankedDocuments(terms);

            
        }
    }

}
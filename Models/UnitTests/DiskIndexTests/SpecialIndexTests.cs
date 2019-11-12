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
            List<string> terms = new List<string>();
            terms.Add("hello");
            terms.Add("world");

            //Testing ranked retrieval AND accumulated values
            index = new DiskPositionalIndex(pathToIndex);
            RankingVariant rv = new RankingVariant(index, corpus);
            IList<MaxPriorityQueue.InvertedIndex> actual = rv.GetRankedDocuments(index, terms, 0);
            actual[0].GetDocumentId().Should().Be(0); //should be document 1 which is of doc id 0
            actual[0].GetRank().Should().BeApproximately(1.183748156, 9); //A_{doccument} = 3.10195041 L_{1} = 2.620447934


        }
    }

}
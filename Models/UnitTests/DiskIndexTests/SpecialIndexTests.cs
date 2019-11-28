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
            Directory.CreateDirectory(pathToIndex);

            //Initialize the index. 
            IIndex index = Indexer.IndexCorpus(corpus);

            

            //The rest of your code...
            List<string> terms = new List<string>();
            terms.Add("hello");
            terms.Add("world");

            //Testing ranked retrieval AND accumulated values
            index = new DiskPositionalIndex(pathToIndex);
            RankedRetrieval rv = new RankedRetrieval(corpus, index, "Default");
            IList<MaxPriorityQueue.InvertedIndex> actual = rv.GetTopTen(terms);
            actual[0].GetDocumentId().Should().Be(0); //should be document 1 which is of doc id 0
            actual[0].GetRank().Should().BeApproximately(1.183748156, 9); //A_{doccument} = 3.10195041 L_{1} = 2.620447934
            actual[1].GetDocumentId().Should().Be(2); //
            actual[2].GetDocumentId().Should().Be(1); //
            actual[3].GetDocumentId().Should().Be(4); //

            //tests tf-idf
            rv = new RankedRetrieval(corpus, index, "Tf-idf");
            IList<MaxPriorityQueue.InvertedIndex> actual1 = rv.GetTopTen(terms);
            actual1[0].GetDocumentId().Should().Be(2);
            actual1[0].GetRank().Should().BeApproximately(0.948215482, 9);
            actual1[1].GetDocumentId().Should().Be(0);
            actual1[1].GetRank().Should().BeApproximately(0.893296803, 9);
            actual1[2].GetDocumentId().Should().Be(1);
            actual1[2].GetRank().Should().BeApproximately(0.150554959, 9);
            actual1[3].GetDocumentId().Should().Be(4);
            actual1[3].GetRank().Should().BeApproximately(0.150554959, 9);


            //tests Okapi BM25
            rv = new RankedRetrieval(corpus, index, "Okapi");
            IList<MaxPriorityQueue.InvertedIndex> actual2 = rv.GetTopTen(terms);
            actual2[0].GetDocumentId().Should().Be(0);
            actual2[0].GetRank().Should().BeApproximately(0.66590893, 9);
            actual2[1].GetDocumentId().Should().Be(2);
            actual2[1].GetRank().Should().BeApproximately(0.507521667, 9);
            actual2[2].GetDocumentId().Should().Be(1);
            actual2[2].GetRank().Should().BeApproximately(0.1089371981, 9);
            actual2[3].GetDocumentId().Should().Be(4);
            actual2[3].GetRank().Should().BeApproximately(0.1084371981, 9);


            //tests Wacky 
            rv = new RankedRetrieval(corpus, index, "Wacky");
            IList<MaxPriorityQueue.InvertedIndex> actual3 = rv.GetTopTen(terms);
            actual3[0].GetDocumentId().Should().Be(0);
            actual3[0].GetRank().Should().BeApproximately(0.284824391, 9);
            actual3[1].GetDocumentId().Should().Be(2);
            actual3[1].GetRank().Should().BeApproximately(0.259673474, 9);
            actual3[2].GetDocumentId().Should().Be(1);
            actual3[2].GetRank().Should().Be(0.0);
            actual3[3].GetDocumentId().Should().Be(4);
            actual3[3].GetRank().Should().Be(0.0);

        }
    }

}
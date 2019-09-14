using System;
using System.Collections.Generic;
using Search.Document;
using Search.Index;
using Xunit;

namespace Search.InvertedIndexer
{
    public class SearchFromIndexTest
    {
        IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./UnitTests/testCorpus", ".txt");
        //Hand-written inverted index
        public Dictionary<string, List<Posting>> CreateValidIndex(){
            Dictionary<string, List<Posting>> validIndex = new Dictionary<string, List<Posting>>();
            validIndex.Add("hello", PostingFactory(new List<int> {0,2}));
            validIndex.Add("world", PostingFactory(new List<int> {0,1,4}));
            validIndex.Add("it",    PostingFactory(new List<int> {0,1,2,3,4}));
            validIndex.Add("is",    PostingFactory(new List<int> {0,1,2,3,4}));
            validIndex.Add("snowing", PostingFactory(new List<int> {0,2}));
            validIndex.Add("the",   PostingFactory(new List<int> {1,3,4}));
            validIndex.Add("full",  PostingFactory(new List<int> {1,4}));
            validIndex.Add("of",    PostingFactory(new List<int> {1,4}));
            validIndex.Add("mystery", PostingFactory(new List<int> {1,3,4}));
            validIndex.Add("snows", PostingFactory(new List<int> {1,4}));
            validIndex.Add("mr.snowman", PostingFactory(new List<int> {2,3}));
            validIndex.Add("loves", PostingFactory(new List<int> {3}));
            validIndex.Add("sun",   PostingFactory(new List<int> {3}));
            validIndex.Add("a",     PostingFactory(new List<int> {3}));
            return validIndex;
        }
        public List<Posting> PostingFactory (List<int> docIdList){
            List<Posting> list = new List<Posting>();
            foreach(int docId in docIdList){
                list.Add(new Posting(docId));
            }
            return list;
        }

        // IIndex index = InvertedIndex
        InvertedIndexer indexer = new InvertedIndexer();
        

        [Fact]
        public void PassingIndexCorpusTest()
        {
            // Assert.Equal(CreateValidIndex(), InvertedIndex.Ind)
        }
    }
}

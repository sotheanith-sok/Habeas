using System.Collections.Generic;
using Search.Document;
using Search.Index;
using Search.InvertedIndexer;
using Xunit;


namespace UnitTests
{
    public class IndexTests
    {
        // test method name convention
        // public void MethodName_Senario_ExpectedBehavior(){
        //     //Arrange
        //     //Act
        //     //Assert
        // }
        

        [Fact]
        public void IndexCorpusTest()
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./UnitTests/testCorpus", ".txt");
            Dictionary<string, List<Posting>> validIndex = CreateValidIndex();

            //Act
            //TODO: Need to find a way to use private method in InvertedIndexer
            // PrivateType pt = new PrivateType(typeof(InvertedIndexer));
            // ...
            // IIndex index = InvertedIndexer.IndexCorpus(corpus);

            //Assert
            // Assert.Equal(validIndex, index);

        }


        //Creates a Hand-written inverted index
        //TODO: Just know the hand-written result of expected index from the testCorpus
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

    }
}

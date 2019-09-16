using System;
using System.Collections.Generic;
using Search.Document;
using Search.Index;
using Search.Text;
using Xunit;

namespace UnitTests
{
    public class IndexTests
    {
        //TODO: why the path was not working with "./UnitTest/testCorpus"?
        static IDocumentCorpus corpus = 
        DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus", ".txt");
        IIndex index = IndexCorpus(corpus);

        // [Theory]
        // [InlineData("hello", 2)]
        // [InlineData("mystery", 3)]
        // public void PostingCountTest(string term, int expected)
        // {
        //     //Arrange & Act
        //     int result = index.GetPostings(term).Count;
        //     Assert.Equal(expected, result);
        // }

        [Theory]
        [MemberData(nameof(Data))]
        public void InvertedIndexTest(string term, List<Posting> expected)
        {
            IList<Posting> result = index.GetPostings(term);

            //Assert
            // TODO: Due to OS difference, can't compare exact List<Posting>
            // the order of docIds assigned to documents is different...
            Assert.Equal(expected.Count, result.Count);
        }
        
        public static TheoryData<string, List<Posting>> Data =>
            new TheoryData<string, List<Posting>> {
                {"hello", new List<Posting>{new Posting(0), new Posting(2)}},
                {"sun", new List<Posting>{new Posting(3)}}
            };


        public static IIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BasicTokenProcessor();
            InvertedIndex index = new InvertedIndex();

            Console.WriteLine("UnitTest: Indexing the corpus... with Inverted Index");
            foreach (IDocument doc in corpus.GetDocuments())
            {
                ITokenStream stream = new EnglishTokenStream(doc.GetContent());
                IEnumerable<string> tokens = stream.GetTokens();

                foreach (string token in tokens) {
                    string term = processor.ProcessToken(token);
                    if(term.Length > 0) {
                        index.AddTerm(term, doc.DocumentId);
                    }
                }
                stream.Dispose();
            }

            return index;
        }
    
    }
}

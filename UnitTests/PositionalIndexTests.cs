using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;
using Search.Text;

//Aren't they too big to call unit tests?


namespace UnitTests
{
    public class PositionalIndexTests {

        [Theory]
        [MemberData(nameof(Data))]
        public void PositoinalPostingTest(string term, List<PositionalPosting> expected)
        {
            //Arrange
            IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("./UnitTests/testCorpus", ".txt");
            PositionalInvertedIndex index;
            IList<PositionalPosting> result;

            //Act
            index = IndexCorpus(corpus);
            result = index.GetPositionalPostings(term);
            
            //Assert
            Assert.Equal(expected.Count, result.Count);
            
        }

        //Test data for positional inverted index
        //Assumed the terms were processed with BasicTokenProcessor
        public static TheoryData<string, List<PositionalPosting>> Data =>
            new TheoryData<string, List<PositionalPosting>> {
                {"hello", new List<PositionalPosting>{
                    new PositionalPosting(0, new List<int>{0,1}),
                    new PositionalPosting(2, new List<int>{0,2,3})
                }},
                {"snows", new List<PositionalPosting>{
                    new PositionalPosting(1, new List<int>{7,8,9}),
                    new PositionalPosting(4, new List<int>{1,2,3})
                }},
                {"sun", new List<PositionalPosting>{
                    new PositionalPosting(3, new List<int>{5})
                }},
            };

        private static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BasicTokenProcessor();

            // Constuct a inverted-index once 
            PositionalInvertedIndex index = new PositionalInvertedIndex();

            System.Console.WriteLine("UnitTest: Indexing the corpus... with Positional Index");
            // Index the document
            foreach (IDocument doc in corpus.GetDocuments())
            {
                //Tokenize the documents
                ITokenStream stream = new EnglishTokenStream(doc.GetContent());
                IEnumerable<string> tokens = stream.GetTokens();

                foreach (string token in tokens) {
                    //Process token to term
                    string term = processor.ProcessToken(token);
                    //Add term to the index
                    if(term.Length > 0) {
                        //TODO: pass a proper position parameter
                        index.AddTerm(term, doc.DocumentId, 0);
                    }
                }
                stream.Dispose();
            }

            return index;
        }


    }

}


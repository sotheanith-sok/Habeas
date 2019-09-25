using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.PositionalInvertedIndexer;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;

//Aren't they too big to call unit tests?

namespace UnitTests
{
    public class PositionalIndexTests
    {

        //Arrange
        static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");
        PositionalInvertedIndex index = IndexCorpus(corpus);
        [Theory]
        [MemberData(nameof(Data))]
        public void PositoinalPostingTest(string term, List<Posting> expected)
        {
            var result = index.GetPostings(term);

            //Assert
            // TODO: Use FluentAssertion. It can check if A contains all in B.
            // Use some assertions other than equal().
            Assert.Equal(expected.Count, result.Count);
            //Assert.Equal(expected, result);

            Console.WriteLine($"term: {term}");
            for (int i = 0; i < Math.Max(expected.Count, result.Count); i++)
            {
                Assert.Equal(expected[i].ToString(), result[i].ToString());
                Console.WriteLine($"expected: {expected[i].ToString()} \t actual: {result[i].ToString()}");
            }

        }

        //Test data for positional inverted index
        //Assumed the terms were processed with BasicTokenProcessor
        //The docID in the data is generated depend on different OS
        public static TheoryData<string, List<Posting>> Data()
        {
            var winData = new TheoryData<string, List<Posting>> {
                {"hello", new List<Posting>{
                    new Posting(0, new List<int>{0,1}),
                    new Posting(2, new List<int>{0,2,3})
                }},
                {"snows", new List<Posting>{
                    new Posting(1, new List<int>{7,8,9}),
                    new Posting(4, new List<int>{1,2,3})
                }},
            };

            var macData = new TheoryData<string, List<Posting>> {
                {"hello", new List<Posting>{
                    new Posting(2, new List<int>{0,2,3}),
                    new Posting(4, new List<int>{0,1}),
                }},
                {"snows", new List<Posting>{
                    new Posting(0, new List<int>{1,2,3}),
                    new Posting(3, new List<int>{7,8,9}),
                }},
            };

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                System.Console.WriteLine("TestData for macOSX");
                return macData;
            }
            else
            {
                System.Console.WriteLine("TestData for other OSs");
                return winData;
            }
        }

        //For independent unit testing, Copied from PositionalInvertedIndexer.IndexCorpus()
        public static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BetterTokenProcessor();
            PositionalInvertedIndex index = new PositionalInvertedIndex();
            Console.WriteLine("UnitTests: Indexing the corpus... with Positional Inverted Index");
            // Index the document
            foreach (IDocument doc in corpus.GetDocuments())
            {
                //Tokenize the documents
                ITokenStream stream = new EnglishTokenStream(doc.GetContent());
                IEnumerable<string> tokens = stream.GetTokens();

                int position = 0;
                foreach (string token in tokens)
                {
                    //Process token to term
                    List<string> terms = processor.ProcessToken(token, false, false);
                    //Add term to the index
                    foreach (string term in terms)
                    {
                        index.AddTerm(term, doc.DocumentId, position);
                    }
                    //Increase the position num
                    position += 1;
                }

                stream.Dispose();
            }

            return index;
        }

    }

}


using Xunit;
using System.Collections.Generic;
using Search.Document;
using Search.Index;
using Search.Text;
using System.Runtime.InteropServices;
using System;
using FluentAssertions;

namespace UnitTests
{
    public class PositionalIndexTests {
        IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");

        [Fact]
        public void PostionalIndexTest_OnePosition(){
            //Arrange
            string term = "sun";
            IList<Posting> expected;
            System.Console.WriteLine("PositionalIndexTest_OnePosition: ");
            System.Console.Write($"Set expected postings of \'{term}\'");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                System.Console.WriteLine(" for MacOSX");
                expected = new List<Posting>{ new Posting(1, new List<int>{3}) };
            }
            else {
                System.Console.WriteLine(" for Windows and other OSs");
                expected = new List<Posting>{ new Posting(3, new List<int>{3}) };
            }

            //Act
            PositionalInvertedIndex index = IndexCorpus(corpus);
            var result = index.GetPostings(term);
            
            //Assert
            index.Should().NotBeNull("because indexCorpus shouldn't return null");
            result.Should().HaveSameCount(expected);
            result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
        }

        [Fact]
        public void PostionalIndexTest_MultiplePositions(){
            //Arrange
            string term = "hello";
            IList<Posting> expected;
            System.Console.WriteLine("PositionalIndexTest_MultiplePositions: ");
            System.Console.Write($"Set expected postings of \'{term}\'");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                System.Console.WriteLine(" for MacOSX");
                expected = new List<Posting>{ new Posting(2, new List<int>{0,2,3}),
                                              new Posting(4, new List<int>{0,1}) };
            } else {
                System.Console.WriteLine(" for Windows and other OSs");
                expected = new List<Posting>{ new Posting(0, new List<int>{0,1}),
                                              new Posting(2, new List<int>{0,2,3}) };
            }

            //Act
            PositionalInvertedIndex index = IndexCorpus(corpus);
            var result = index.GetPostings(term);
            
            //Assert
            index.Should().NotBeNull("because indexCorpus shouldn't return null");
            result.Should().HaveSameCount(expected);
            result.Should().BeEquivalentTo(expected, config => config.WithStrictOrdering());
        }


        //For independent unit testing, Copied from PositionalInvertedIndexer.IndexCorpus()
        public static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new NormalTokenProcessor();
            PositionalInvertedIndex index = new PositionalInvertedIndex();
            Console.WriteLine($"UnitTest: Indexing {corpus.CorpusSize} documents in the corpus...");
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
                    List<string> terms = processor.ProcessToken(token);
                    //Add term to the index
                    foreach (string term in terms)
                    {
                        if (term.Length > 0)
                        {
                            index.AddTerm(term, doc.DocumentId, position);
                        }
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


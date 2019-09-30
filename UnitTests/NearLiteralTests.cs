using Xunit;
using FluentAssertions;
using System;
using Search.Query;
using Search.Document;
using Search.Index;
using Search.Text;
using System.Collections.Generic;

namespace UnitTests
{
    public class NearLiteralTests
    {
        static IDocumentCorpus corpus = DirectoryCorpus.LoadTextDirectory("../../../UnitTests/testCorpus");
        IIndex index = IndexCorpus(corpus);

        [Fact]
        public void GetPostingsTest_NearExist_ReturnsSomePos()
        {
            //Test for exact words
            NearLiteral near = new NearLiteral("is", 4, "mystery"); //[is NEAR/4 mystery]
            IList<Posting> result = near.GetPostings(index);
            // IList<Posting> expected = MergeTests.GeneratePostings("...");
            result.Should().HaveCount(3);
            Console.Write(near.ToString()+"\t"); PrintPostingResult(result);

            //Test for stemmed words
            //TODO: Should the terms be processed and stemmed??
            //so that the [it NEAR/2 snowing] search can include [it NEAR/2 snow] and [it NEAR/2 snows]?
            NearLiteral near2 = new NearLiteral("it", 2, "snowing");   //[it NEAR/2 snowing]
            result = near2.GetPostings(index);
            // result.Should().HaveCount(4, "because processed \'snowing\' should include result of \'snow\' and \'snows\'");
            Console.Write(near2.ToString()+"\t"); PrintPostingResult(result);
        }
        
        [Fact]
        public void GetPostingsTest_NearNotExist_ReturnsEmpty()
        {
            NearLiteral near = new NearLiteral("is", 1, "mystery"); //[is NEAR/1 mystery]
            IList<Posting> result = near.GetPostings(index);

            result.Should().BeEmpty("because there's no document that \'mystery\' appears 1 away from \'is\'");
        }


        private void PrintPostingResult(IList<Posting> result) {
            foreach(Posting p in result) {
                Console.Write(p.ToString() + "  ");
            }
            Console.WriteLine();
        }

        //For independent unit testing, Copied from PositionalInvertedIndexer.IndexCorpus()
        private static PositionalInvertedIndex IndexCorpus(IDocumentCorpus corpus)
        {
            ITokenProcessor processor = new BetterTokenProcessor();
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
                    List<string> terms = processor.ProcessToken(token, false, false);
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
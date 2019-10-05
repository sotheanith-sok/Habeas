using Xunit;
using FluentAssertions;
using Search.Query;
using System.Collections.Generic;

namespace UnitTests
{
    public class ParseQueryTest
    {
        [Fact]
        public void ParseTest_SingleQuery_ReturnsTermLiteral() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "smoothie";
            TermLiteral expected = new TermLiteral(query);
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(TermLiteral));
            ((TermLiteral)actual).Term.Should().BeSameAs(expected.Term);
        }

        [Fact]
        public void ParseTest_AndQuery_ReturnsAndQuery() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "kiwi banana";
            AndQuery expected = new AndQuery(
                    new List<IQueryComponent>{
                        new TermLiteral("kiwi"), new TermLiteral("banana")
                    }
                );
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(AndQuery));
            ((AndQuery)actual).Components.Should().HaveSameCount(expected.Components);
        }

        [Fact]
        public void ParseTest_OrQuery_ReturnsOrQuery() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "kiwi + banana";
            OrQuery expected = new OrQuery(
                    new List<IQueryComponent>{
                        new TermLiteral("kiwi"), new TermLiteral("banana")
                    }
                );
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(OrQuery));
            ((OrQuery)actual).Components.Should().HaveSameCount(expected.Components);
        }

        [Fact]
        public void ParseTest_PhraseQuery_ReturnsPhraseLiteral() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "\"ice smoothie\"";
            PhraseLiteral expected = new PhraseLiteral("ice smoothie");
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(PhraseLiteral));
            ((PhraseLiteral)actual).ToString().Should().BeEquivalentTo(expected.ToString());

            //Case2: query without ending quote
            query = "\"ice smoothie";
            actual = parser.ParseQuery(query);
            actual.Should().BeOfType(typeof(PhraseLiteral));
            ((PhraseLiteral)actual).ToString().Should().BeEquivalentTo(expected.ToString());
        }

        [Fact]
        public void ParseCompoundQueryTest_OrAndQuery() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "kiwi + banana mango apple + cherry tomato";
            OrQuery expected = new OrQuery(
                    new List<IQueryComponent>{
                        new TermLiteral("kiwi"),
                        new AndQuery( new List<IQueryComponent>{
                            new TermLiteral("banana"),
                            new TermLiteral("mango"),
                            new TermLiteral("apple")
                        }),
                        new AndQuery( new List<IQueryComponent>{
                            new TermLiteral("cherry"),
                            new TermLiteral("tomato")
                        })
                    });
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            //check or-query
            actual.Should().BeOfType(typeof(OrQuery));
            ((OrQuery)actual).Components.Should().HaveCount(3);
            //check and-query
            ((OrQuery)actual).Components[0].Should().BeOfType(typeof(TermLiteral));
            ((OrQuery)actual).Components[1].Should().BeOfType(typeof(AndQuery));
            ((AndQuery)((OrQuery)actual).Components[1]).Components.Should().HaveCount(3);
            ((OrQuery)actual).Components[2].Should().BeOfType(typeof(AndQuery));
            ((AndQuery)((OrQuery)actual).Components[2]).Components.Should().HaveCount(2);
        }

        [Fact]
        public void ParseTest_NearQuery_ReturnsNearLiteral() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "[lemon NEAR/2 orange]";
            NearLiteral expected = new NearLiteral("lemon", 2, "orange");
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(NearLiteral));
            ((NearLiteral)actual).ToString().Should().BeEquivalentTo(expected.ToString());
        }

        //TODO: wildcard literal
        // [Fact]
        // public void ParseTest_WildcardQuery_ReturnsWildcardLiteral() {
        //     //Arrange
        //     BooleanQueryParser parser = new BooleanQueryParser();
        //     string query = "colo*r";
        //     WildcardLiteral expected = new WildcardLiteral(query);
        //     //Act
        //     IQueryComponent actual = parser.ParseQuery(query);
        //     //Assert
        //     actual.Should().BeOfType(typeof(WildcardLiteral));
        //     ((WildcardLiteral)actual).Should().BeEquivalentTo(expected);
        // }


        //TODO: test compound query parsing with everything
        [Fact]
        public void ParseCompoundQueryTest_Everything() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "str*berry kiwi + banana \"ice smoothie\" party + [lemon NEAR/2 orange]";
            // OrQuery expected = new OrQuery(
            //         new List<IQueryComponent>{
            //             new AndQuery( new List<IQueryComponent>{
            //                 new WildcardLiteral("str*berry"),
            //                 new TermLiteral("kiwi")
            //             }),
            //             new AndQuery( new List<IQueryComponent>{
            //                 new TermLiteral("banana"),
            //                 new PhraseLiteral("ice smoothie"),
            //                 new TermLiteral("party")
            //             }),
            //             new NearLiteral("lemon",2,"orange")
            //         }
            //     );
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            //actual.Should()...
        }
    }
}
using Xunit;
using FluentAssertions;
using Search.Query;
using System.Collections.Generic;

namespace UnitTests
{
    public class ParseQueryTest
    {
        [Fact]
        public void ParseTest_SingleQuery() {
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
        public void ParseTest_AndQuery() {
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
        public void ParseTest_OrQuery() {
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
        public void ParseTest_PhraseQuery() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "\"banana smoothie\"";
            PhraseLiteral expected = new PhraseLiteral("banana smoothie");
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(PhraseLiteral));
            ((PhraseLiteral)actual).ToString().Should().BeEquivalentTo(expected.ToString());
        }

        //TODO: near literal
        // [Fact]
        // public void ParseTest_NearLiteral() {
        //     //Arrange
        //     BooleanQueryParser parser = new BooleanQueryParser();
        //     string query = "[lemon NEAR/2 orange]";
        //     NearLiteral expected = new NearLiteral(query);
        //     //Act
        //     IQueryComponent actual = parser.ParseQuery(query);
        //     //Assert
        //     actual.Should().BeOfType(typeof(NearLiteral));
        //     ((NearLiteral)actual).Should().BeEquivalentTo(expected);
        // }

        //TODO: wildcard literal
        // [Fact]
        // public void ParseTest_WildcardLiteral() {
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

        //TODO: compound query
        [Fact]
        public void ParseTest_CompoundQueries() {
            //Arrange
            BooleanQueryParser parser = new BooleanQueryParser();
            string query = "kiwi + banana \"ice smoothie\" party";
            OrQuery expected = new OrQuery(
                    new List<IQueryComponent>{
                        new TermLiteral("kiwi"),
                        new AndQuery( new List<IQueryComponent>{
                            new TermLiteral("banana"),
                            new PhraseLiteral("ice smoothie"),
                            new TermLiteral("party")
                        })
                    }
                );
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(OrQuery));
            //TODO: how can I check this compound query component
            actual.Should().BeEquivalentTo(expected);
        }

    }
}
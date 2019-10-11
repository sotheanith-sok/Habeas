using Xunit;
using FluentAssertions;
using Search.Query;
using System.Collections.Generic;
using Search.Index;

namespace UnitTests
{
    /// <summary>
    /// Tests if BooleanQueryParser create appropriate query components from a query string
    /// </summary>
    public class ParserTests
    {
        BooleanQueryParser parser = new BooleanQueryParser();

        [Fact]
        public void ParsingSingleQueryTest_ReturnsTermLiteral()
        {
            //Arrange
            string query = "smoothie";
            TermLiteral expected = new TermLiteral(query);
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(TermLiteral));
            ((TermLiteral)actual).Term.Should().BeSameAs(expected.Term);
        }

        [Fact]
        public void ParsingAndQueryTest_ReturnsAndQuery()
        {
            //Arrange
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
        public void ParsingOrQueryTest_ReturnsOrQuery()
        {
            //Arrange
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
        public void ParsingPhraseQueryTest_ReturnsPhraseLiteral()
        {
            //Arrange
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
        public void ParsingNearQueryTest_ReturnsNearLiteral()
        {
            //Arrange
            string query = "[lemon NEAR/2 orange]";
            NearLiteral expected = new NearLiteral("lemon", 2, "orange");
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(NearLiteral));
            ((NearLiteral)actual).ToString().Should().BeEquivalentTo(expected.ToString());
        }

        [Fact]
        public void ParseTest_WildcardQuery_ReturnsWildcardLiteral()
        {
            //Arrange
            string query = "colo*r";
            //Act
            IQueryComponent actual = parser.ParseQuery(query);
            //Assert
            actual.Should().BeOfType(typeof(WildcardLiteral));
        }

        [Fact]
        public void ParsingTest_WithOrAnd()
        {
            //Arrange
            string query = "kiwi + banana mango apple + cherry tomato";
            // Expected structure: OR( Term, AND(Term,Term,Term), AND(Term,Term) )

            //Act
            IQueryComponent actual = parser.ParseQuery(query);

            //Assert
            //check level1 or-query
            actual.Should().BeOfType(typeof(OrQuery));
            ((OrQuery)actual).Components.Should().HaveCount(3);
            //check level2 and-query
            ((OrQuery)actual).Components[0].Should().BeOfType(typeof(TermLiteral));
            ((OrQuery)actual).Components[1].Should().BeOfType(typeof(AndQuery));
            ((AndQuery)((OrQuery)actual).Components[1]).Components.Should().HaveCount(3);
            ((OrQuery)actual).Components[2].Should().BeOfType(typeof(AndQuery));
            ((AndQuery)((OrQuery)actual).Components[2]).Components.Should().HaveCount(2);
        }

        [Fact]
        public void ParseTest_WithEverything()
        {
            //Arrange
            string query = "str*berry kiwi + banana \"ice smoothie\" party + [lemon NEAR/2 orange]";
            // Expected structure: OR( AND(Wildcard,Term), AND(Term,Phrase,Term), NEAR )

            //Act
            IQueryComponent actual = parser.ParseQuery(query);

            //Assert
            //check level1 or-query
            actual.Should().BeOfType(typeof(OrQuery));
            ((OrQuery)actual).Components.Should().HaveCount(3);
            //check level2
            ((OrQuery)actual).Components[0].Should().BeOfType(typeof(AndQuery));
            ((AndQuery)((OrQuery)actual).Components[0]).Components.Should().HaveCount(2);
            ((OrQuery)actual).Components[1].Should().BeOfType(typeof(AndQuery));
            ((AndQuery)((OrQuery)actual).Components[1]).Components.Should().HaveCount(3);
            ((OrQuery)actual).Components[2].Should().BeOfType(typeof(NearLiteral));
            //check level3
            ((AndQuery)((OrQuery)actual).Components[0]).Components[0].Should().BeOfType(typeof(WildcardLiteral));
            ((AndQuery)((OrQuery)actual).Components[0]).Components[1].Should().BeOfType(typeof(TermLiteral));
            ((AndQuery)((OrQuery)actual).Components[1]).Components[0].Should().BeOfType(typeof(TermLiteral));
            ((AndQuery)((OrQuery)actual).Components[1]).Components[1].Should().BeOfType(typeof(PhraseLiteral));
            ((AndQuery)((OrQuery)actual).Components[1]).Components[2].Should().BeOfType(typeof(TermLiteral));
        }

        [Fact]
        public void ParsingTest_Exception_ReturnsNull()     //TODO: other cases to be handled?
        {
            //with empty string
            string query = "";
            IQueryComponent actual = parser.ParseQuery(query);
            actual.Should().BeNull();

            // query = "  ";
            query = null;
            actual = parser.ParseQuery(query);
            actual.Should().BeNull();
        }
    }
}
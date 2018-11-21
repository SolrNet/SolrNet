using System;
using System.Collections.Generic;
using Xunit;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests
{
    public class MoneyTests
    {

        public static IEnumerable<object[]> moneys = new[] {
               new  object[] { new Money(12, null),  "12" },
                new object[] {  new Money(12.45m, "USD"),  "12.45,USD", },
                new object[] {  new Money(52.66m, "EUR"),  "52.66,EUR", },
            };


        [Theory]
        [MemberData(nameof(moneys))]
        public void ToStringRepresentation(Money money, string stringRepresentation)
        {
            Assert.Equal(stringRepresentation, money.ToString());
        }

        [Theory]
        [MemberData(nameof(moneys))]
        public void Parse(Money money, string stringRepresentation)
        {
            var parsedMoney = MoneyFieldParser.Parse(stringRepresentation);
            Assert.Equal(money.Currency, parsedMoney.Currency);
            Assert.Equal(money.Value, parsedMoney.Value);

        }

    }
}


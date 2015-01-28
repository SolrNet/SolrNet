using System;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    public static class MoneyTests {
        [StaticTestFactory]
        public static IEnumerable<Test> Tests() {
            var moneys = new[] {
                new { money = new Money(12, null), toString = "12", },
                new { money = new Money(12.45m, "USD"), toString = "12.45,USD", },
                new { money = new Money(52.66m, "EUR"), toString = "52.66,EUR", },
            };

            foreach (var m in moneys) {
                var x = m;
                yield return new TestCase("ToString " + x.toString, 
                    () => Assert.AreEqual(x.toString, x.money.ToString()));

                yield return new TestCase("Parse " + x.toString, () => {
                    var parsedMoney = MoneyFieldParser.Parse(x.toString);
                    Assert.AreEqual(x.money.Currency, parsedMoney.Currency);
                    Assert.AreEqual(x.money.Value, parsedMoney.Value);
                });
            }

        }
    }
}

using System;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    public static class LocationTests {
        [StaticTestFactory]
        public static IEnumerable<Test> Tests() {
            var locations = new[] {
                new { location = new Location(12, 23), toString = "12,23" },
                new { location = new Location(-4.3, 0.20), toString = "-4.3,0.2" },
            };

            foreach (var l in locations) {
                var x = l;
                yield return new TestCase("ToString " + x.toString,
                    () => Assert.AreEqual(x.toString, x.location.ToString()));

                yield return new TestCase("Parse " + x.toString, () => {
                    var parsedLocation = LocationFieldParser.Parse(x.toString);
                    Assert.AreEqual(x.location, parsedLocation);
                    Assert.AreEqual(x.location.Latitude, parsedLocation.Latitude);
                    Assert.AreEqual(x.location.Longitude, parsedLocation.Longitude);
                });
            }
        }
    }
}

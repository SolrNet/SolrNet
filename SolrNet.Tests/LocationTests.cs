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

            var invalidLatitudes = new[] {-100, 120};
            foreach (var x in invalidLatitudes) {
                var latitude = x;

                yield return new TestCase("Latitude " + latitude + " is invalid", 
                    () => Assert.IsFalse(Location.IsValidLatitude(latitude)));

                yield return new TestCase("Invalid latitude throws: " + latitude, 
                    () => Assert.Throws<ArgumentOutOfRangeException>(() => new Location(latitude, 0)));
            }

            var invalidLongitudes = new[] {-200, 999};
            foreach (var x in invalidLongitudes) {
                var longitude = x;

                yield return new TestCase("Longitude " + longitude + " is invalid",
                    () => Assert.IsFalse(Location.IsValidLongitude(longitude)));

                yield return new TestCase("Invalid longitude throws: " + longitude,
                    () => Assert.Throws<ArgumentOutOfRangeException>(() => new Location(0, longitude)));
            }

            yield return new TestCase("TryCreate returns null with invalid lat/long", () => {
                foreach (var lat in invalidLatitudes)
                    foreach (var lng in invalidLongitudes) {
                        var loc = Location.TryCreate(lat, lng);
                        Assert.IsNull(loc);
                    }
            });

            yield return new TestCase("TryCreate returns non-null with valid lat/long", () => {
                foreach (var l in locations) {
                    var loc = l.location;
                    var loc2 = Location.TryCreate(loc.Latitude, loc.Longitude);
                    Assert.IsNotNull(loc2);
                    Assert.AreEqual(loc, loc2);
                }
            });
        }
    }
}

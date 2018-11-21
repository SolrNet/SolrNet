using System;
using System.Collections.Generic;
using Xunit;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests
{
    public class LocationTests
    {
        public static IEnumerable<object[]> locations = new[] {
                new object[] {new Location(12, 23), "12,23" },
                new object[] {new Location(-4.3, 0.20), "-4.3,0.2" },
            };


        [Theory]
        [MemberData(nameof(locations))]
        public void ToStringRepresentation(Location location, string stringRepresentation)
        {
            Assert.Equal(stringRepresentation, location.ToString());
        }

        [Theory]
        [MemberData(nameof(locations))]
        public void Parse(Location location, string stringRepresentation)
        {
            var parsedLocation = LocationFieldParser.Parse(stringRepresentation);
            Assert.Equal(location, parsedLocation);
            Assert.Equal(location.Latitude, parsedLocation.Latitude);
            Assert.Equal(location.Longitude, parsedLocation.Longitude);
        }


        public static IEnumerable<object[]> invalidLatitutes = new[] { new object[] { -100 }, new object[] { -120 } };
        [Theory]
        [MemberData(nameof(invalidLatitutes))]
        public void InvalidLatitude(int latitude)
        {

            Assert.False(Location.IsValidLatitude(latitude));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Location(latitude, 0));

        }

        public static IEnumerable<object[]> invalidLongitudes = new[] { new object[] { -200 }, new object[] { 999 } };
        [Theory]
        [MemberData(nameof(invalidLongitudes))]
        public void InvalidLongitudes(int longitude)
        {

            Assert.False(Location.IsValidLongitude(longitude));
            Assert.Throws<ArgumentOutOfRangeException>(() => new Location(0, longitude));

        }

        public static IEnumerable<object[]> invalidLatLong = new[] { new object[] { -100, -200 }, new object[] { -120, 999 } };
        [Theory]
        [MemberData(nameof(invalidLatLong))]
        public void InvalidLatLong(int lat, int lng)
        {
            var loc = Location.TryCreate(lat, lng);
            Assert.Null(loc);

        }


        [Theory]
        [MemberData(nameof(locations))]
        public void ValidLatLong(Location location, string stringRepresentation)
        {
            var loc2 = Location.TryCreate((double)location.Latitude, (double)location.Longitude);
            Assert.NotNull(loc2);
            Assert.Equal((Location)location, loc2);
        }

    }
}

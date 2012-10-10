using System;
using System.Globalization;

namespace SolrNet {
    public class Location : IEquatable<Location> {
        public readonly double Latitude;
        public readonly double Longitude;

        public Location(double latitude, double longitude) {
            Latitude = latitude;
            Longitude = longitude;
        }

        public override string ToString() {
            return string.Format(CultureInfo.InvariantCulture, "{0},{1}", Latitude, Longitude);
        }

        public override bool Equals(object obj) {
            var other = obj as Location;
            if (other == null)
                return false;
            return Equals(other);
        }

        public override int GetHashCode() {
            return new { Latitude, Longitude }.GetHashCode();
        }

        public bool Equals(Location other) {
            if (other == null)
                return false;
            return other.Latitude == Latitude &&
                other.Longitude == Longitude;
        }
    }
}

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

        public bool Equals(Location other) {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((Location) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (Latitude.GetHashCode()*397) ^ Longitude.GetHashCode();
            }
        }
    }
}

using System;
using System.Globalization;

namespace SolrNet {
    /// <summary>
    /// Represents a Latitude/Longitude as a 2 dimensional point. 
    /// </summary>
    public class Location : IEquatable<Location>, IFormattable {
        /// <summary>
        /// Latitude
        /// </summary>
        public readonly double Latitude;

        /// <summary>
        /// Longitude
        /// </summary>
        public readonly double Longitude;

        /// <summary>
        /// Represents a Latitude/Longitude as a 2 dimensional point. 
        /// </summary>
        /// <param name="latitude">Value between -90 and 90</param>
        /// <param name="longitude">Value between -180 and 180</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="latitude"/> or <paramref name="longitude"/> are invalid</exception>
        public Location(double latitude, double longitude) {
            if (!IsValidLatitude(latitude))
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Invalid latitude '{0}'. Valid values are between -90 and 90", latitude));
            if (!IsValidLongitude(longitude))
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, "Invalid longitude '{0}'. Valid values are between -180 and 180", longitude));
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// True if <paramref name="latitude"/> is a valid latitude. Otherwise false.
        /// </summary>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public static bool IsValidLatitude(double latitude) {
            return latitude >= -90 && latitude <= 90;
        }

        /// <summary>
        /// True if <paramref name="longitude"/> is a valid longitude. Otherwise false.
        /// </summary>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static bool IsValidLongitude(double longitude) {
            return longitude >= -180 && longitude <= 180;
        }

        /// <summary>
        /// Try to create a <see cref="Location"/>. 
        /// Return <value>null</value> if either <paramref name="latitude"/> or <paramref name="longitude"/> are invalid.
        /// </summary>
        /// <param name="latitude">Value between -90 and 90</param>
        /// <param name="longitude">Value between -180 and 180</param>
        /// <returns></returns>
        public static Location TryCreate(double latitude, double longitude) {
            if (IsValidLatitude(latitude) && IsValidLongitude(longitude))
                return new Location(latitude, longitude);
            return null;
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

        public string ToString(string format, IFormatProvider formatProvider) {
            return ToString();
        }
    }
}

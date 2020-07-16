// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;

namespace Azure.Core.Spatial
{
    /// <summary>
    ///
    /// </summary>
    public readonly struct GeographyCoordinate
    {
        /// <summary>
        /// Gets the altitude of the position.
        /// </summary>
        public double? Altitude { get; }

        /// <summary>
        /// Gets the longitude of the position.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Gets the latitude of the position.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="GeoCoordinate"/>.
        /// </summary>
        /// <param name="longitude">The longitude of the position.</param>
        /// <param name="latitude">The latitude of the position.</param>
        public GeographyCoordinate(double longitude, double latitude) : this(longitude, latitude, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="GeoCoordinate"/>.
        /// </summary>
        /// <param name="longitude">The longitude of the position.</param>
        /// <param name="latitude">The latitude of the position.</param>
        /// <param name="altitude">The altitude of the position.</param>
        public GeographyCoordinate(double longitude, double latitude, double? altitude)
        {
            Longitude = longitude;
            Latitude = latitude;
            Altitude = altitude;
        }


        /// <inheritdoc />
        public bool Equals(GeographyCoordinate other)
        {
            return Nullable.Equals(Altitude, other.Altitude) && Longitude.Equals(other.Longitude) && Latitude.Equals(other.Latitude);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is GeographyCoordinate other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeBuilder.Combine(Longitude, Latitude, Altitude);

        /// <summary>
        /// Determines whether two specified positions have the same value.
        /// </summary>
        /// <param name="left">The first position to compare.</param>
        /// <param name="right">The first position to compare.</param>
        /// <returns><c>true</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(GeographyCoordinate left, GeographyCoordinate right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two specified positions have the same value.
        /// </summary>
        /// <param name="left">The first position to compare.</param>
        /// <param name="right">The first position to compare.</param>
        /// <returns><c>false</c> if the value of <c>left</c> is the same as the value of <c>b</c>; otherwise, <c>true</c>.</returns>
        public static bool operator !=(GeographyCoordinate left, GeographyCoordinate right)
        {
            return !left.Equals(right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (Altitude == null)
            {
                return $"Longitude: {Longitude:G}, Latitude: {Latitude:G}";
            }

            return $"Longitude: {Longitude:G}, Latitude: {Latitude:G}, Altitude: {Altitude.Value:G}";
        }
    }
}
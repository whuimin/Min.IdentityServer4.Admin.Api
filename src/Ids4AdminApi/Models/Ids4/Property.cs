// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
    /// <summary>
    /// Models a property.
    /// </summary>
    public class Property
    {
        /// <summary>
        /// Unique ID of data row.
        /// </summary>
        public int Id { get; set; }

		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		[StringLength(250, MinimumLength = 1, ErrorMessage = "The {0} value must be between {2} and {1} character in length.")]
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		[StringLength(2000, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Value { get; set; }

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 17;
				hash = hash * 23 + (Key?.GetHashCode() ?? 0);
				hash = hash * 23 + (Value?.GetHashCode() ?? 0);

				return hash;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
		/// <returns>
		///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj == null) return false;
			if (obj is not Secret other) return false;
			if (ReferenceEquals(other, this)) return true;

			return string.Equals(other.Type, Key, StringComparison.Ordinal) &&
				string.Equals(other.Value, Value, StringComparison.Ordinal);
		}
	}
}
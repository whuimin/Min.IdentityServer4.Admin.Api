// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models the common data of API and identity resources.
	/// </summary>
	public abstract class Resource
	{
		/// <summary>
		/// Unique ID of data row.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Indicates if this resource is enabled. Defaults to true.
		/// </summary>
		public bool Enabled { get; set; } = true;

		/// <summary>
		/// The unique name of the resource.
		/// </summary>
		[StringLength(200, MinimumLength = 1, ErrorMessage = "The {0} value must be between {2} and {1} character in length.")]
		public string Name { get; set; }

		/// <summary>
		/// Display name of the resource.
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Description of the resource.
		/// </summary>
		[StringLength(1000, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Description { get; set; }

		/// <summary>
		/// Specifies whether this scope is shown in the discovery document. Defaults to true.
		/// </summary>
		public bool ShowInDiscoveryDocument { get; set; } = true;

		/// <summary>
		/// List of associated user claims that should be included when this resource is requested.
		/// </summary>
		public ICollection<string> UserClaims { get; set; } = new HashSet<string>();

		/// <summary>
		/// Gets or sets the custom properties for the resource.
		/// </summary>
		/// <value>
		/// The properties.
		/// </value>
		public ICollection<Property> Properties { get; set; } = new HashSet<Property>();
	}
}
using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a user identity resource list input parameters.
	/// </summary>
	public class IdentityResourceListInput
	{
		/// <summary>
		/// Specifies if identity resource is enabled.
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Unique name of the identity resource.
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Name { get; set; }

		/// <summary>
		/// Display name of the identity resource.
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Description of the identity resource.
		/// </summary>
		[StringLength(1000, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the create time begin. (>=)
		/// </summary>
		public DateTime? CreatedBegin { get; set; }

		/// <summary>
		/// Gets or sets the create time end. (&lt;)
		/// </summary>
		public DateTime? CreatedEnd { get; set; }

		/// <summary>
		/// Gets or sets the update time begin. (>=)
		/// </summary>
		public DateTime? UpdatedBegin { get; set; }

		/// <summary>
		/// Gets or sets the update time end. (&lt;)
		/// </summary>
		public DateTime? UpdatedEnd { get; set; }
	}
}

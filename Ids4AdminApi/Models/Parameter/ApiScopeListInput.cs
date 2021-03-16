using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models access to an API scope list input parameters.
	/// </summary>
	public class ApiScopeListInput
	{
		/// <summary>
		/// Specifies if API scope is enabled.
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Unique name of the API scope.
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Name { get; set; }

		/// <summary>
		/// Display name of the API scope.
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Description of the API scope.
		/// </summary>
		[StringLength(1000, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Description { get; set; }
	}
}

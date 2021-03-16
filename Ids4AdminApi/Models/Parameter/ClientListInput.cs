using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models an OpenID Connect or OAuth2 client list input parameters.
	/// </summary>
	public class ClientListInput
	{
		/// <summary>
		/// Specifies if client is enabled.
		/// </summary>
		public bool? Enabled { get; set; }

		/// <summary>
		/// Unique ID of the client.
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string ClientId { get; set; }

		/// <summary>
		/// Client display name (used for logging and consent screen).
		/// </summary>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string ClientName { get; set; }

		/// <summary>
		/// Description of the client.
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

		/// <summary>
		/// Gets or sets the last access time begin. (>=)
		/// </summary>
		public DateTime? LastAccessedBegin { get; set; }

		/// <summary>
		/// Gets or sets the last access time end. (&lt;)
		/// </summary>
		public DateTime? LastAccessedEnd { get; set; }
	}
}

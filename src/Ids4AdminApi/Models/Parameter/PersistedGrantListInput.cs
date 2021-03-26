using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// A model for a persisted grant list input parameters.
	/// </summary>
	public class PersistedGrantListInput
	{
		/// <summary>
		/// Gets or sets the key.
		/// </summary>
		/// <value>
		/// The key.
		/// </value>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Key { get; set; }

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>
		/// The type.
		/// </value>
		[StringLength(50, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Type { get; set; }

		/// <summary>
		/// Gets the subject identifier.
		/// </summary>
		/// <value>
		/// The subject identifier.
		/// </value>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string SubjectId { get; set; }

		/// <summary>
		/// Gets the session identifier.
		/// </summary>
		/// <value>
		/// The session identifier.
		/// </value>
		[StringLength(100, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string SessionId { get; set; }

		/// <summary>
		/// Gets the client identifier.
		/// </summary>
		/// <value>
		/// The client identifier.
		/// </value>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string ClientId { get; set; }

		/// <summary>
		/// Gets the description the user assigned to the device being authorized.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		[StringLength(200, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the creation time begin. (>=)
		/// </summary>
		public DateTime? CreationTimeBegin { get; set; }

		/// <summary>
		/// Gets or sets the creation time end. (&lt;)
		/// </summary>
		public DateTime? CreationTimeEnd { get; set; }

		/// <summary>
		/// Gets or sets the expiration time begin. (>=)
		/// </summary>
		public DateTime? ExpirationBegin { get; set; }

		/// <summary>
		/// Gets or sets the expiration time end. (&lt;)
		/// </summary>
		public DateTime? ExpirationEnd { get; set; }

		/// <summary>
		/// Gets or sets the consumed time begin. (>=)
		/// </summary>
		public DateTime? ConsumedTimeBegin { get; set; }

		/// <summary>
		/// Gets or sets the consumed time end. (&lt;)
		/// </summary>
		public DateTime? ConsumedTimeEnd { get; set; }

		/// <summary>
		/// Gets or sets the data.
		/// </summary>
		/// <value>
		/// The data.
		/// </value>
		[StringLength(100, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
		public string Data { get; set; }
	}
}

using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Represents a API request header.
	/// </summary>
	public class RequestHeader
	{
		/// <summary>
		///  Gets or sets the version of API request.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int Version { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of API request.
		/// </summary>
		public string RequestId { get; set; } = Guid.NewGuid().ToString("N");

		/// <summary>
		/// Gets or sets the culture name of API request.
		/// </summary>
		public string Culture { get; set; } = CultureName.en_US;
	}
}

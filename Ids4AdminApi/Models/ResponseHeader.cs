using System.Collections.Generic;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Represents a API response header.
	/// </summary>
	public class ResponseHeader
	{
		/// <summary>
		/// Gets or sets the version of API reponse. Match with the version of API request.
		/// </summary>
		public int Version { get; set; }

		/// <summary>
		/// Gets or sets the unique identifier of API reponse. Match with the unique identifier of API request.
		/// </summary>
		public string ResponseId { get; set; }

		/// <summary>
		/// Gets or sets the result of API response.
		/// </summary>
		public bool IsSuccess { get; set; }

		/// <summary>
		/// Gets or sets the error list of API response.
		/// </summary>
		public IEnumerable<ResponseError> Errors { get; set; }
	}
}

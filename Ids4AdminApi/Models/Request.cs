using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Represents a API request object.
	/// </summary>
	public class Request
	{
		/// <summary>
		/// Gets or sets the API request header.
		/// </summary>
		[Required]
		public virtual RequestHeader Header { get; set; }
	}

	/// <summary>
	/// Represents a API request object with generic type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Request<T> : Request
	{
		/// <summary>
		/// Gets or sets the API request header.
		/// </summary>
		[Required]
		public override RequestHeader Header { get; set; }

		/// <summary>
		/// Gets or sets the API request body.
		/// </summary>
		public T Body { get; set; }
	}
}

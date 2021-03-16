namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Represents a API response object.
	/// </summary>
	public class Response
	{
		/// <summary>
		/// Gets or sets the API response header.
		/// </summary>
		public virtual ResponseHeader Header { get; set; }
	}

	/// <summary>
	/// Represents a API response object with generic type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Response<T> : Response
	{
		/// <summary>
		/// Gets or sets the API response header.
		/// </summary>
		public override ResponseHeader Header { get; set; }

		/// <summary>
		/// Gets or sets the API response body.
		/// </summary>
		public T Body { get; set; }
	}
}

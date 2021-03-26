namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a T type view output result.
	/// </summary>
	public class ViewOutput<T> where T : class
	{
		/// <summary>
		/// Gets or sets the output data.
		/// </summary>
		public T Data { get; set; }
	}
}

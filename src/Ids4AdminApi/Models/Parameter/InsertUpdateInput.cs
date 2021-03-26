using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a T type insert update input parameters.
	/// </summary>
	public class InsertUpdateInput<T> where T : class
	{
		/// <summary>
		/// Gets or sets the input data.
		/// </summary>
		[Required]
		public T Data { get; set; }
	}
}

using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a unique ID input parameters.
	/// </summary>
	public class UniqueIdInput
	{
		/// <summary>
		/// Unique ID of data row.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int Id { get; set; }
	}
}

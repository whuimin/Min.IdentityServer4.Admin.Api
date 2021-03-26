using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a unique key input parameters.
	/// </summary>
	public class UniqueKeyInput
	{
		/// <summary>
		/// Unique key of data row.
		/// </summary>
		[Required]
		public string Key { get; set; }
	}
}

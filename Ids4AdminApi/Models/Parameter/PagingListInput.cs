using System;
using System.ComponentModel.DataAnnotations;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a paging list input parameters.
	/// </summary>
	public class PagingListInput<TCondition> where TCondition : class
	{
		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		[Range(1, int.MaxValue)]
		public int PageNo { get; set; }

		/// <summary>
		/// Gets or sets the page size.
		/// </summary>
		[Range(10, int.MaxValue)]
		public int PageSize { get; set; }

		/// <summary>
		/// Gets or sets the search condition.
		/// </summary>
		[Required]
		public TCondition Condition { get; set; }
	}
}

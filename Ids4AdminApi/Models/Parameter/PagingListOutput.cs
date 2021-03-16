using System.Collections.Generic;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a paging list output parameters.
	/// </summary>
	public class PagingListOutput<TData> where TData : class
	{
		/// <summary>
		/// Gets or sets the page number.
		/// </summary>
		public int PageNo { get; set; }

		/// <summary>
		/// Gets or sets the page size.
		/// </summary>
		public int PageSize { get; set; }

		/// <summary>
		/// Gets or sets the total record count.
		/// </summary>
		public int TotalCount { get; set; }

		/// <summary>
		/// Gets or sets the data list.
		/// </summary>
		public IEnumerable<TData> DataList { get; set; }
	}
}

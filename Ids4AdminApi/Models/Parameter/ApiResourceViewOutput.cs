using System.Collections.Generic;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models a web API resource view output result.
	/// </summary>
	public class ApiResourceViewOutput : ViewOutput<ApiResource>
	{
		/// <summary>
		/// Gets or sets the all available scopes.
		/// </summary>
		public HashSet<string> AllScopes { get; set; }
	}
}

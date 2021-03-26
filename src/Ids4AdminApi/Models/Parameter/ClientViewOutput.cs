using System.Collections.Generic;

namespace Ids4AdminApi.Models
{
	/// <summary>
	/// Models an OpenID Connect or OAuth2 client view output result.
	/// </summary>
	public class ClientViewOutput : ViewOutput<Client>
	{
		/// <summary>
		/// Gets or sets the all available scopes.
		/// </summary>
		public HashSet<string> AllScopes { get; set; }
	}
}

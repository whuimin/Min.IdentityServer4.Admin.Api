using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	class RedirectUrisResolver :
		IValueResolver<Models.Client, Client, List<ClientRedirectUri>>,
		IValueResolver<Models.Client, Client, List<ClientPostLogoutRedirectUri>>
	{
		public List<ClientRedirectUri> Resolve(Models.Client source, Client destination, List<ClientRedirectUri> destMember, ResolutionContext context)
		{
			var entityList = new List<ClientRedirectUri>();
			if (source.AllowedGrantTypes == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.RedirectUri) ?? new Dictionary<string, ClientRedirectUri>();
			foreach (var item in source.RedirectUris)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ClientRedirectUri>(item));
				}
				else
				{
					entityList.Add(entity);
				}
			}

			return entityList;
		}

		public List<ClientPostLogoutRedirectUri> Resolve(Models.Client source, Client destination, List<ClientPostLogoutRedirectUri> destMember, ResolutionContext context)
		{
			var entityList = new List<ClientPostLogoutRedirectUri>();
			if (source.PostLogoutRedirectUris == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.PostLogoutRedirectUri) ?? new Dictionary<string, ClientPostLogoutRedirectUri>();
			foreach (var item in source.PostLogoutRedirectUris)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ClientPostLogoutRedirectUri>(item));
				}
				else
				{
					entityList.Add(entity);
				}
			}

			return entityList;
		}
	}
}

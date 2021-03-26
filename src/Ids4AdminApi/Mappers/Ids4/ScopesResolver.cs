using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	class ScopesResolver :
		IValueResolver<Models.Client, Client, List<ClientScope>>,
		IValueResolver<Models.ApiResource, ApiResource, List<ApiResourceScope>>
	{
		public List<ClientScope> Resolve(Models.Client source, Client destination, List<ClientScope> destMember, ResolutionContext context)
		{
			var entityList = new List<ClientScope>();
			if (source.AllowedGrantTypes == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Scope) ?? new Dictionary<string, ClientScope>();
			foreach (var item in source.AllowedScopes)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ClientScope>(item));
				}
				else
				{
					entityList.Add(entity);
				}
			}

			return entityList;
		}

		public List<ApiResourceScope> Resolve(Models.ApiResource source, ApiResource destination, List<ApiResourceScope> destMember, ResolutionContext context)
		{
			var entityList = new List<ApiResourceScope>();
			if (source.Scopes == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Scope) ?? new Dictionary<string, ApiResourceScope>();
			foreach (var item in source.Scopes)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ApiResourceScope>(item));
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

using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	class IdentityProviderRestrictionsResolver : IValueResolver<Models.Client, Client, List<ClientIdPRestriction>>
	{
		public List<ClientIdPRestriction> Resolve(Models.Client source, Client destination, List<ClientIdPRestriction> destMember, ResolutionContext context)
		{
			var entityList = new List<ClientIdPRestriction>();
			if (source.IdentityProviderRestrictions == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Provider) ?? new Dictionary<string, ClientIdPRestriction>();
			foreach (var item in source.IdentityProviderRestrictions)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ClientIdPRestriction>(item));
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

using System.Collections.Generic;
using System.Linq;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	class AllowedGrantTypesResolver : IValueResolver<Models.Client, Client, List<ClientGrantType>>
	{
		public List<ClientGrantType> Resolve(Models.Client source, Client destination, List<ClientGrantType> destMember, ResolutionContext context)
		{
			var entityList = new List<ClientGrantType>();
			if (source.AllowedGrantTypes == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.GrantType) ?? new Dictionary<string, ClientGrantType>();
			foreach (var item in source.AllowedGrantTypes)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ClientGrantType>(item));
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

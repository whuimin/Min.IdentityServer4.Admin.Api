using System.Collections.Generic;
using System.Linq;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	class AllowedCorsOriginsResolver : IValueResolver<Models.Client, Client, List<ClientCorsOrigin>>
	{
		public List<ClientCorsOrigin> Resolve(Models.Client source, Client destination, List<ClientCorsOrigin> destMember, ResolutionContext context)
		{
			var entityList = new List<ClientCorsOrigin>();
			if (source.AllowedCorsOrigins == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Origin) ?? new Dictionary<string, ClientCorsOrigin>();
			foreach (var item in source.AllowedCorsOrigins)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{
					
					entityList.Add(context.Mapper.Map<ClientCorsOrigin>(item));
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

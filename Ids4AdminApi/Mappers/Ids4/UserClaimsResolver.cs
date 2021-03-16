using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	class UserClaimsResolver :
		IValueResolver<Models.ApiScope, ApiScope, List<ApiScopeClaim>>,
		IValueResolver<Models.ApiResource, ApiResource, List<ApiResourceClaim>>,
		IValueResolver<Models.IdentityResource, IdentityResource, List<IdentityResourceClaim>>
	{
		public List<ApiScopeClaim> Resolve(Models.ApiScope source, ApiScope destination, List<ApiScopeClaim> destMember, ResolutionContext context)
		{
			var entityList = new List<ApiScopeClaim>();
			if (source.UserClaims == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Type) ?? new Dictionary<string, ApiScopeClaim>();
			foreach (var item in source.UserClaims)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ApiScopeClaim>(item));
				}
				else
				{
					entityList.Add(entity);
				}
			}

			return entityList;
		}

		public List<ApiResourceClaim> Resolve(Models.ApiResource source, ApiResource destination, List<ApiResourceClaim> destMember, ResolutionContext context)
		{
			var entityList = new List<ApiResourceClaim>();
			if (source.UserClaims == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Type) ?? new Dictionary<string, ApiResourceClaim>();
			foreach (var item in source.UserClaims)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<ApiResourceClaim>(item));
				}
				else
				{
					entityList.Add(entity);
				}
			}

			return entityList;
		}

		public List<IdentityResourceClaim> Resolve(Models.IdentityResource source, IdentityResource destination, List<IdentityResourceClaim> destMember, ResolutionContext context)
		{
			var entityList = new List<IdentityResourceClaim>();
			if (source.UserClaims == null)
			{
				return entityList;
			}
			var entityMapping = destMember?.ToDictionary(x => x.Type) ?? new Dictionary<string, IdentityResourceClaim>();
			foreach (var item in source.UserClaims)
			{
				var entity = entityMapping.GetValueOrDefault(item);
				if (entity == null)
				{

					entityList.Add(context.Mapper.Map<IdentityResourceClaim>(item));
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

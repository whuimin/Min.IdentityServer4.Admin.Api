// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Defines entity/model mapping for scopes.
	/// </summary>
	/// <seealso cref="AutoMapper.Profile" />
	public class ApiScopeMapperProfile : Profile
	{
		/// <summary>
		/// <see cref="ApiScopeMapperProfile"/>
		/// </summary>
		public ApiScopeMapperProfile()
		{
			CreateMap<Entities.ApiScopeProperty, Models.Property>()
				.ReverseMap();

			CreateMap<Entities.ApiScopeClaim, string>()
			   .ConstructUsing(x => x.Type)
			   .ReverseMap()
			   .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ApiScope, Models.ApiScope>(MemberList.Destination)
				.ConstructUsing(src => new Models.ApiScope())
				.ForMember(x => x.Properties, opts => opts.MapFrom(x => x.Properties))
				.ForMember(x => x.UserClaims, opts => opts.MapFrom(x => x.UserClaims))
				.ReverseMap()
				.ForMember(x => x.UserClaims, opts => opts.MapFrom<UserClaimsResolver>());
		}
	}
}

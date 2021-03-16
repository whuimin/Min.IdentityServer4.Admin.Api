﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Defines entity/model mapping for identity resources.
	/// </summary>
	/// <seealso cref="AutoMapper.Profile" />
	public class IdentityResourceMapperProfile : Profile
	{
		/// <summary>
		/// <see cref="IdentityResourceMapperProfile"/>
		/// </summary>
		public IdentityResourceMapperProfile()
		{
			CreateMap<Entities.IdentityResourceProperty, Models.Property>()
				.ReverseMap();

			CreateMap<Entities.IdentityResourceClaim, string>()
			   .ConstructUsing(x => x.Type)
			   .ReverseMap()
			   .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

			CreateMap<Entities.IdentityResource, Models.IdentityResource>(MemberList.Destination)
				.ConstructUsing(src => new Models.IdentityResource())
				.ReverseMap()
				.ForMember(x => x.UserClaims, opts => opts.MapFrom<UserClaimsResolver>());
		}
	}
}

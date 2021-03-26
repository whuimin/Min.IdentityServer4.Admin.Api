// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Defines entity/model mapping for clients.
	/// </summary>
	/// <seealso cref="AutoMapper.Profile" />
	public class ClientMapperProfile : Profile
	{
		/// <summary>
		/// <see>
		///     <cref>{ClientMapperProfile}</cref>
		/// </see>
		/// </summary>
		public ClientMapperProfile()
		{
			CreateMap<Entities.ClientProperty, Models.Property>()
				.ReverseMap();

			CreateMap<Entities.ClientGrantType, string>()
				.ConstructUsing(src => src.GrantType)
				.ReverseMap()
				.ForMember(dest => dest.GrantType, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ClientRedirectUri, string>()
				.ConstructUsing(src => src.RedirectUri)
				.ReverseMap()
				.ForMember(dest => dest.RedirectUri, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ClientPostLogoutRedirectUri, string>()
				.ConstructUsing(src => src.PostLogoutRedirectUri)
				.ReverseMap()
				.ForMember(dest => dest.PostLogoutRedirectUri, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ClientScope, string>()
				.ConstructUsing(src => src.Scope)
				.ReverseMap()
				.ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ClientIdPRestriction, string>()
				.ConstructUsing(src => src.Provider)
				.ReverseMap()
				.ForMember(dest => dest.Provider, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ClientCorsOrigin, string>()
				.ConstructUsing(src => src.Origin)
				.ReverseMap()
				.ForMember(dest => dest.Origin, opt => opt.MapFrom(src => src));

			CreateMap<Entities.ClientClaim, Models.ClientClaim>(MemberList.None)
				.ConstructUsing(src => new Models.ClientClaim(src.Type, src.Value, ClaimValueTypes.String))
				.ReverseMap();

			CreateMap<Entities.ClientSecret, Models.Secret>(MemberList.Destination)
				.ForMember(dest => dest.Type, opt => opt.Condition(srs => srs != null))
				.ReverseMap();

			CreateMap<Entities.Client, Models.Client>()
				.ForMember(dest => dest.ProtocolType, opt => opt.Condition(srs => srs != null))
				.ForMember(x => x.AllowedIdentityTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedIdentityTokenSigningAlgorithms))
				.ReverseMap()
				.ForMember(x => x.AllowedIdentityTokenSigningAlgorithms, opts => opts.ConvertUsing(AllowedSigningAlgorithmsConverter.Converter, x => x.AllowedIdentityTokenSigningAlgorithms))
				.ForMember(x => x.AllowedGrantTypes, opts => opts.MapFrom<AllowedGrantTypesResolver>())
				.ForMember(x => x.RedirectUris, opts => opts.MapFrom<RedirectUrisResolver>())
				.ForMember(x => x.PostLogoutRedirectUris, opts => opts.MapFrom<RedirectUrisResolver>())
				.ForMember(x => x.AllowedScopes, opts => opts.MapFrom<ScopesResolver>())
				.ForMember(x => x.IdentityProviderRestrictions, opts => opts.MapFrom<IdentityProviderRestrictionsResolver>())
				.ForMember(x => x.AllowedCorsOrigins, opts => opts.MapFrom<AllowedCorsOriginsResolver>());
		}
	}
}
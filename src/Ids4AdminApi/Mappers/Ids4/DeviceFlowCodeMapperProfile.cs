// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Defines entity/model mapping for device flow codes.
	/// </summary>
	/// <seealso cref="AutoMapper.Profile" />
	public class DeviceFlowCodeMapperProfile : Profile
	{
		/// <summary>
		/// <see>
		///     <cref>{ClientMapperProfile}</cref>
		/// </see>
		/// </summary>
		public DeviceFlowCodeMapperProfile()
		{
			CreateMap<Entities.DeviceFlowCodes, Models.DeviceFlowCode>(MemberList.Destination)
				.ReverseMap();
		}
	}
}
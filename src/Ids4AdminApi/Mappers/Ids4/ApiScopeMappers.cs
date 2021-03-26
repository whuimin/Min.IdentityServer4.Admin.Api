// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Extension methods to map to/from entity/model for scopes.
	/// </summary>
	public static class ApiScopeMappers
	{
		static ApiScopeMappers()
		{
			Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiScopeMapperProfile>())
				.CreateMapper();
		}

		internal static IMapper Mapper { get; }

		/// <summary>
		/// Maps an entity to a model.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static Models.ApiScope ToModel(this ApiScope entity)
		{
			return entity == null ? null : Mapper.Map<Models.ApiScope>(entity);
		}

		/// <summary>
		/// Maps an entities to a models.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <returns></returns>
		public static IEnumerable<Models.ApiScope> ToModels(this IEnumerable<ApiScope> entities)
		{
			return entities.Select(entity => Mapper.Map<Models.ApiScope>(entity));
		}

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static ApiScope ToEntity(this Models.ApiScope model)
		{
			return model == null ? null : Mapper.Map<ApiScope>(model);
		}

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="entity">The entity model.</param>
		/// <returns></returns>
		public static ApiScope UpdateEntity(this Models.ApiScope model, ApiScope entity)
		{
			return model == null ? entity : Mapper.Map(model, entity);
		}
	}
}
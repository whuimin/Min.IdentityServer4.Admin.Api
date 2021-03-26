// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Extension methods to map to/from entity/model for API resources.
	/// </summary>
	public static class ApiResourceMappers
	{
		static ApiResourceMappers()
		{
			Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
				.CreateMapper();
		}

		internal static IMapper Mapper { get; }

		/// <summary>
		/// Maps an entity to a model.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static Models.ApiResource ToModel(this ApiResource entity)
		{
			return entity == null ? null : Mapper.Map<Models.ApiResource>(entity);
		}

		/// <summary>
		/// Maps an entities to a models.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <returns></returns>
		public static IEnumerable<Models.ApiResource> ToModels(this IEnumerable<ApiResource> entities)
		{
			return entities.Select(entity => Mapper.Map<Models.ApiResource>(entity));
		}

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static ApiResource ToEntity(this Models.ApiResource model)
		{
			return model == null ? null : Mapper.Map<ApiResource>(model);
		}

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="entity">The entity model.</param>
		/// <returns></returns>
		public static ApiResource UpdateEntity(this Models.ApiResource model, ApiResource entity)
		{
			return model == null ? entity : Mapper.Map(model, entity);
		}
	}
}
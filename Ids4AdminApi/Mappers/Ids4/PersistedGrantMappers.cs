// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

using Ids4AdminApi.Models;

namespace Ids4AdminApi.Mappers
{
	/// <summary>
	/// Extension methods to map to/from entity/model for persisted grants.
	/// </summary>
	public static class PersistedGrantMappers
	{
		static PersistedGrantMappers()
		{
			Mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantMapperProfile>())
				.CreateMapper();
		}

		internal static IMapper Mapper { get; }

		/// <summary>
		/// Maps an entity to a model.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		public static PersistedGrant ToModel(this Entities.PersistedGrant entity)
		{
			return entity == null ? null : Mapper.Map<PersistedGrant>(entity);
		}

		/// <summary>
		/// Maps an entities to a models.
		/// </summary>
		/// <param name="entities">The entities.</param>
		/// <returns></returns>
		public static IEnumerable<Models.PersistedGrant> ToModels(this IEnumerable<Entities.PersistedGrant> entities)
		{
			return entities.Select(entity => Mapper.Map<Models.PersistedGrant>(entity));
		}

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public static Entities.PersistedGrant ToEntity(this PersistedGrant model)
		{
			return model == null ? null : Mapper.Map<Entities.PersistedGrant>(model);
		}

		/// <summary>
		/// Updates an entity from a model.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="entity">The entity.</param>
		public static Entities.PersistedGrant UpdateEntity(this PersistedGrant model, Entities.PersistedGrant entity)
		{
			return model == null ? entity : Mapper.Map(model, entity);
		}
	}
}
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class ClientMappers
    {
        static ClientMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.Client ToModel(this Entities.Client entity)
        {
            return Mapper.Map<Models.Client>(entity);
        }

        /// <summary>
        /// Maps an entities to a models.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public static IEnumerable<Models.Client> ToModels(this IEnumerable<Entities.Client> entities)
        {
            return entities.Select(entity=> Mapper.Map<Models.Client>(entity));
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.Client ToEntity(this Models.Client model)
        {
            return Mapper.Map<Entities.Client>(model);
        }

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="entity">The entity model.</param>
		/// <returns></returns>
		public static Entities.Client UpdateEntity(this Models.Client model, Entities.Client entity)
        {
            return Mapper.Map(model, entity);
        }
    }
}
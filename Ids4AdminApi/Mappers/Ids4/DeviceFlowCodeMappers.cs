using System.Linq;
using System.Collections.Generic;

using AutoMapper;
using Entities = IdentityServer4.EntityFramework.Entities;

namespace Ids4AdminApi.Mappers
{
    /// <summary>
    /// Extension methods to map to/from entity/model for device flow codes.
    /// </summary>
    public static class DeviceFlowCodeMappers
    {
        static DeviceFlowCodeMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<DeviceFlowCodeMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Models.DeviceFlowCode ToModel(this Entities.DeviceFlowCodes entity)
        {
            return Mapper.Map<Models.DeviceFlowCode>(entity);
        }

        /// <summary>
        /// Maps an entities to a models.
        /// </summary>
        /// <param name="entities">The entities.</param>
        /// <returns></returns>
        public static IEnumerable<Models.DeviceFlowCode> ToModels(this IEnumerable<Entities.DeviceFlowCodes> entities)
        {
            return entities.Select(entity=> Mapper.Map<Models.DeviceFlowCode>(entity));
        }

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.DeviceFlowCodes ToEntity(this Models.DeviceFlowCode model)
        {
            return model == null ? null : Mapper.Map<Entities.DeviceFlowCodes>(model);
        }

		/// <summary>
		/// Maps a model to an entity.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="entity">The entity model.</param>
		/// <returns></returns>
		public static Entities.DeviceFlowCodes ToEntity(this Models.DeviceFlowCode model, Entities.DeviceFlowCodes entity)
        {
            return model == null ? entity : Mapper.Map(model, entity);
        }
    }
}
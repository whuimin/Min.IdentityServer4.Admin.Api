using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using IdentityServer4.EntityFramework.Interfaces;
using Entities = IdentityServer4.EntityFramework.Entities;

using Ids4AdminApi.Models;
using Ids4AdminApi.Mappers;
using Ids4AdminApi.Extensions;

namespace Ids4AdminApi.Controllers
{
	/// <summary>
	/// Identity resource operation API。
	/// </summary>
	public class IdentityResourceController : BaseController
	{
		private readonly ILogger<IdentityResourceController> logger;
		private readonly IConfigurationDbContext configurationDbContext;

		#region Private method

		/// <summary>
		/// Get Identity resource entity from database.
		/// </summary>
		/// <param name="id">Unique ID of data row</param>
		/// <returns></returns>
		private async Task<Entities.IdentityResource> GetIdentityResource(int id)
		{
			var query = configurationDbContext.IdentityResources.Where(c => c.Id == id);

			await query.Include(x => x.UserClaims).SelectMany(c => c.UserClaims).LoadAsync();
			await query.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();

			var entityIdentityResource = await query.FirstOrDefaultAsync();
			return entityIdentityResource;
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configurationDbContext"></param>
		public IdentityResourceController(ILogger<IdentityResourceController> logger, IConfigurationDbContext configurationDbContext)
		{
			this.logger = logger;
			this.configurationDbContext = configurationDbContext;
		}

		/// <summary>
		/// Return response object with Identity resource list that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<PagingListOutput<IdentityResource>> List([FromBody] Request<PagingListInput<IdentityResourceListInput>> request)
		{
			var response = new Response<PagingListOutput<IdentityResource>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var condition = request.Body.Condition;
			var query = configurationDbContext.IdentityResources.AsQueryable();
			if (condition.Enabled.HasValue)
			{
				query = query.Where(c => c.Enabled == condition.Enabled);
			}

			if (!string.IsNullOrEmpty(condition.Name))
			{
				query = query.Where(c => c.Name == condition.Name);
			}

			if (!string.IsNullOrEmpty(condition.DisplayName))
			{
				query = query.Where(c => c.DisplayName.Contains(condition.DisplayName));
			}

			if (!string.IsNullOrEmpty(condition.Description))
			{
				query = query.Where(c => c.Description.Contains(condition.Description));
			}

			if (condition.CreatedBegin.HasValue)
			{
				query = query.Where(c => c.Created >= condition.CreatedBegin);
			}
			if (condition.CreatedEnd.HasValue)
			{
				query = query.Where(c => c.Created < condition.CreatedEnd);
			}

			if (condition.UpdatedBegin.HasValue)
			{
				query = query.Where(c => c.Updated >= condition.UpdatedBegin);
			}
			if (condition.UpdatedEnd.HasValue)
			{
				query = query.Where(c => c.Updated < condition.UpdatedEnd);
			}

			try
			{
				var pageNo = request.Body.PageNo;
				var pageSize = request.Body.PageSize;

				var skip = (pageNo - 1) * pageSize;
				var entityIdentityResources = query.OrderBy(c => c.Id).Skip(skip).Take(pageSize);
				response.Body = new PagingListOutput<IdentityResource>
				{
					PageNo = pageNo,
					PageSize = pageSize,
					TotalCount = query.Count(),
					DataList = entityIdentityResources.ToModels()
				};
			}
			catch (Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}

		/// <summary>
		/// Return response object with Identity resource that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<IdentityResource>>> View([FromBody] Request<UniqueIdInput> request)
		{
			var response = new Response<ViewOutput<IdentityResource>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			try
			{
				var entityIdentityResource = await GetIdentityResource(request.Body.Id);
				if (entityIdentityResource == null)
				{
					AttachError(response.Header, ResultCode.IdentityResourceNotExist, $"IdentityResource with data row id: {request.Body.Id} is not exist.");
					return response;
				}

				response.Body = new ViewOutput<IdentityResource>
				{
					Data = entityIdentityResource.ToModel()
				};
			}
			catch (Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}

		/// <summary>
		/// Return response object with Identity resource that initialized。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<ViewOutput<IdentityResource>> Init([FromBody] Request request)
		{
			var response = new Response<ViewOutput<IdentityResource>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var client = new IdentityResource();

			response.Body = new ViewOutput<IdentityResource>
			{
				Data = client
			};
			return response;
		}

		/// <summary>
		/// Return response object with Identity resource that inserted by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<IdentityResource>>> Insert([FromBody] Request<InsertUpdateInput<IdentityResource>> request)
		{
			var response = new Response<ViewOutput<IdentityResource>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newIdentityResource = request.Body.Data;
			newIdentityResource.Created = DateTime.UtcNow;
			newIdentityResource.Updated = null;

			var entityIdentityResource = await configurationDbContext.IdentityResources.FirstOrDefaultAsync(c => c.Name == newIdentityResource.Name);
			if (entityIdentityResource != null)
			{
				AttachError(response.Header, ResultCode.IdentityResourceExist, "IdentityResource name is used by another client.");
				return response;
			}

			entityIdentityResource = newIdentityResource.ToEntity();

			try
			{
				var entityEntry = await configurationDbContext.IdentityResources.AddAsync(entityIdentityResource);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ViewOutput<IdentityResource>
				{
					Data = entityEntry.Entity.ToModel()
				};
			}
			catch (Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}

		/// <summary>
		/// Return response object with Identity resource that updated by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<IdentityResource>>> Update([FromBody] Request<InsertUpdateInput<IdentityResource>> request)
		{
			var response = new Response<ViewOutput<IdentityResource>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newIdentityResource = request.Body.Data;
			var entityIdentityResource = await GetIdentityResource(newIdentityResource.Id);
			if (entityIdentityResource == null)
			{
				AttachError(response.Header, ResultCode.IdentityResourceNotExist, $"IdentityResource with data row id: {newIdentityResource.Id} is not exist.");
				return response;
			}

			var isExist = await configurationDbContext.IdentityResources.AnyAsync(c => c.Id != newIdentityResource.Id && c.Name == newIdentityResource.Name);
			if (isExist)
			{
				AttachError(response.Header, ResultCode.IdentityResourceExist, "IdentityResource name is used by another client.");
				return response;
			}

			newIdentityResource.Created = entityIdentityResource.Created;
			newIdentityResource.Updated = DateTime.UtcNow;

			entityIdentityResource = newIdentityResource.UpdateEntity(entityIdentityResource);

			try
			{
				var entityEntry = configurationDbContext.IdentityResources.Update(entityIdentityResource);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ViewOutput<IdentityResource>
				{
					Data = entityEntry.Entity.ToModel()
				};
			}
			catch (Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}

		/// <summary>
		/// Return response object with Identity resource that deleted by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response> Delete([FromBody] Request<UniqueIdInput> request)
		{
			var response = new Response
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			try
			{
				var entityIdentityResource = await GetIdentityResource(request.Body.Id);
				if (entityIdentityResource != null)
				{
					configurationDbContext.IdentityResources.Remove(entityIdentityResource);
					await (configurationDbContext as DbContext).SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}
	}
}

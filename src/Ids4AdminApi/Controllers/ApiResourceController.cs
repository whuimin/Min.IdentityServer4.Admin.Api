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
	/// API resource operation API。
	/// </summary>
	public class ApiResourceController : BaseController
	{
		private readonly ILogger<ApiResourceController> logger;
		private readonly IConfigurationDbContext configurationDbContext;

		#region Private method

		/// <summary>
		/// Get API resource entity from database.
		/// </summary>
		/// <param name="id">Unique ID of data row</param>
		/// <returns></returns>
		private async Task<Entities.ApiResource> GetApiResource(int id)
		{
			var query = configurationDbContext.ApiResources.Where(c => c.Id == id);

			await query.Include(x => x.Scopes).SelectMany(c => c.Scopes).LoadAsync();
			await query.Include(x => x.Secrets).SelectMany(c => c.Secrets).LoadAsync();
			await query.Include(x => x.UserClaims).SelectMany(c => c.UserClaims).LoadAsync();
			await query.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();

			var entityApiResource = await query.FirstOrDefaultAsync();
			return entityApiResource;
		}

		/// <summary>
		/// Get all available scopes from database.
		/// </summary>
		/// <returns></returns>
		private HashSet<string> GetAllScopes()
		{
			var query = configurationDbContext.ApiScopes.Select(a => a.Name);
			var allScopes = query.ToHashSet();

			return allScopes;
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configurationDbContext"></param>
		public ApiResourceController(ILogger<ApiResourceController> logger, IConfigurationDbContext configurationDbContext)
		{
			this.logger = logger;
			this.configurationDbContext = configurationDbContext;
		}

		/// <summary>
		/// Return response object with API resource list that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<PagingListOutput<ApiResource>> List([FromBody] Request<PagingListInput<ApiResourceListInput>> request)
		{
			var response = new Response<PagingListOutput<ApiResource>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var condition = request.Body.Condition;
			var query = configurationDbContext.ApiResources.AsQueryable();
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

			if (condition.LastAccessedBegin.HasValue)
			{
				query = query.Where(c => c.LastAccessed >= condition.LastAccessedBegin);
			}
			if (condition.LastAccessedEnd.HasValue)
			{
				query = query.Where(c => c.LastAccessed < condition.LastAccessedEnd);
			}

			try
			{
				var pageNo = request.Body.PageNo;
				var pageSize = request.Body.PageSize;

				var skip = (pageNo - 1) * pageSize;
				var entityApiResources = query.OrderBy(c => c.Id).Skip(skip).Take(pageSize);
				response.Body = new PagingListOutput<ApiResource>
				{
					PageNo = pageNo,
					PageSize = pageSize,
					TotalCount = query.Count(),
					DataList = entityApiResources.ToModels()
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
		/// Return response object with API resource that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ApiResourceViewOutput>> View([FromBody] Request<UniqueIdInput> request)
		{
			var response = new Response<ApiResourceViewOutput>
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
				var entityApiResource = await GetApiResource(request.Body.Id);
				if (entityApiResource == null)
				{
					AttachError(response.Header, ResultCode.ApiResourceNotExist, $"ApiResource with data row id: {request.Body.Id} is not exist.");
					return response;
				}

				response.Body = new ApiResourceViewOutput
				{
					Data = entityApiResource.ToModel(),
					AllScopes = GetAllScopes()
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
		/// Return response object with API resource that initialized。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<ApiResourceViewOutput> Init([FromBody] Request request)
		{
			var response = new Response<ApiResourceViewOutput>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var apiResource = new ApiResource();
			apiResource.ApiSecrets.Add(new Secret(Guid.NewGuid().ToString("N").Substring(0, 8)));

			response.Body = new ApiResourceViewOutput
			{
				Data = apiResource,
				AllScopes = GetAllScopes()
			};
			return response;
		}

		/// <summary>
		/// Return response object with API resource that inserted by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ApiResourceViewOutput>> Insert([FromBody] Request<InsertUpdateInput<ApiResource>> request)
		{
			var response = new Response<ApiResourceViewOutput>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newApiResource = request.Body.Data;
			newApiResource.Created = DateTime.UtcNow;
			newApiResource.Updated = null;
			newApiResource.LastAccessed = null;
			foreach (var secret in newApiResource.ApiSecrets)
			{
				secret.Value = secret.Value.Sha512();
				secret.Created = DateTime.UtcNow;
			}

			var entityApiResource = await configurationDbContext.ApiResources.FirstOrDefaultAsync(c => c.Name == newApiResource.Name);
			if (entityApiResource != null)
			{
				AttachError(response.Header, ResultCode.ApiResourceExist, "ApiResource name is used by another API resource.");
				return response;
			}

			entityApiResource = newApiResource.ToEntity();

			try
			{
				var entityEntry = await configurationDbContext.ApiResources.AddAsync(entityApiResource);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ApiResourceViewOutput
				{
					Data = entityEntry.Entity.ToModel(),
					AllScopes = GetAllScopes()
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
		/// Return response object with API resource that updated by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ApiResourceViewOutput>> Update([FromBody] Request<InsertUpdateInput<ApiResource>> request)
		{
			var response = new Response<ApiResourceViewOutput>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newApiResource = request.Body.Data;
			var entityApiResource = await GetApiResource(newApiResource.Id);
			if (entityApiResource == null)
			{
				AttachError(response.Header, ResultCode.ApiResourceNotExist, $"ApiResource with data row id: {newApiResource.Id} is not exist.");
				return response;
			}

			var isExist = await configurationDbContext.ApiResources.AnyAsync(c => c.Id != newApiResource.Id && c.Name == newApiResource.Name);
			if (isExist)
			{
				AttachError(response.Header, ResultCode.ApiResourceExist, "ApiResource name is used by another API resource.");
				return response;
			}

			newApiResource.Created = entityApiResource.Created;
			newApiResource.Updated = DateTime.UtcNow;
			newApiResource.LastAccessed = entityApiResource.LastAccessed;
			var entityApiResourceMapping = entityApiResource.Secrets?.ToDictionary(cs => cs.Id) ?? new Dictionary<int, Entities.ApiResourceSecret>();
			foreach (var secret in newApiResource.ApiSecrets)
			{
				var entitySecret = entityApiResourceMapping.GetValueOrDefault(secret.Id);
				if (entitySecret == null || entitySecret.Value != secret.Value)
				{
					secret.Value = secret.Value.Sha512();
					secret.Created = DateTime.UtcNow;
				}
			}

			entityApiResource = newApiResource.UpdateEntity(entityApiResource);

			try
			{
				var entityEntry = configurationDbContext.ApiResources.Update(entityApiResource);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ApiResourceViewOutput
				{
					Data = entityEntry.Entity.ToModel(),
					AllScopes = GetAllScopes()
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
		/// Return response object with API resource that deleted by using input parameter。
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
				var entityApiResource = await GetApiResource(request.Body.Id);
				if (entityApiResource != null)
				{
					configurationDbContext.ApiResources.Remove(entityApiResource);
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

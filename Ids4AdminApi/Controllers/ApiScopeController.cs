using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using IdentityServer4.EntityFramework.Interfaces;
using Entities = IdentityServer4.EntityFramework.Entities;

using Ids4AdminApi.Models;
using Ids4AdminApi.Mappers;

namespace Ids4AdminApi.Controllers
{
	/// <summary>
	/// API scope operation API。
	/// </summary>
	public class ApiScopeController : BaseController
	{
		private readonly ILogger<ApiScopeController> logger;
		private readonly IConfigurationDbContext configurationDbContext;

		#region Private method

		/// <summary>
		/// Get API scope entity from database.
		/// </summary>
		/// <param name="id">Unique ID of data row</param>
		/// <returns></returns>
		private async Task<Entities.ApiScope> GetApiScope(int id)
		{
			var query = configurationDbContext.ApiScopes.Where(c => c.Id == id);

			await query.Include(x => x.UserClaims).SelectMany(c => c.UserClaims).LoadAsync();
			await query.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();

			var entityApiScope = await query.FirstOrDefaultAsync();
			return entityApiScope;
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configurationDbContext"></param>
		public ApiScopeController(ILogger<ApiScopeController> logger, IConfigurationDbContext configurationDbContext)
		{
			this.logger = logger;
			this.configurationDbContext = configurationDbContext;
		}

		/// <summary>
		/// Return response object with API scope list that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<PagingListOutput<ApiScope>> List([FromBody] Request<PagingListInput<ApiScopeListInput>> request)
		{
			var response = new Response<PagingListOutput<ApiScope>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var condition = request.Body.Condition;
			var query = configurationDbContext.ApiScopes.AsQueryable();
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

			try
			{
				var pageNo = request.Body.PageNo;
				var pageSize = request.Body.PageSize;

				var skip = (pageNo - 1) * pageSize;
				var entityApiScopes = query.OrderBy(c => c.Id).Skip(skip).Take(pageSize);
				response.Body = new PagingListOutput<ApiScope>
				{
					PageNo = pageNo,
					PageSize = pageSize,
					TotalCount = query.Count(),
					DataList = entityApiScopes.ToModels()
				};
			}
			catch(Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}

		/// <summary>
		/// Return response object with API scope that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<ApiScope>>> View([FromBody] Request<UniqueIdInput> request)
		{
			var response = new Response<ViewOutput<ApiScope>>
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
				var entityApiScope = await GetApiScope(request.Body.Id);
				if (entityApiScope == null)
				{
					AttachError(response.Header, ResultCode.ApiScopeNotExist, $"ApiScope with data row id: {request.Body.Id} is not exist.");
					return response;
				}

				response.Body = new ViewOutput<ApiScope>
				{
					Data = entityApiScope.ToModel()
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
		/// Return response object with API scope that initialized。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<ViewOutput<ApiScope>> Init([FromBody] Request request)
		{
			var response = new Response<ViewOutput<ApiScope>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var apiScope = new ApiScope();

			response.Body = new ViewOutput<ApiScope>
			{
				Data = apiScope
			};
			return response;
		}

		/// <summary>
		/// Return response object with API scope that inserted by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<ApiScope>>> Insert([FromBody] Request<InsertUpdateInput<ApiScope>> request)
		{
			var response = new Response<ViewOutput<ApiScope>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newApiScope = request.Body.Data;
			var entityApiScope = await configurationDbContext.ApiScopes.FirstOrDefaultAsync(c => c.Name == newApiScope.Name);
			if (entityApiScope != null)
			{
				AttachError(response.Header, ResultCode.ApiScopeExist, "ApiScope name is used by another API scope.");
				return response;
			}

			entityApiScope = newApiScope.ToEntity();

			try
			{
				var entityEntry = await configurationDbContext.ApiScopes.AddAsync(entityApiScope);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ViewOutput<ApiScope>
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
		/// Return response object with API scope that updated by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<ApiScope>>> Update([FromBody] Request<InsertUpdateInput<ApiScope>> request)
		{
			var response = new Response<ViewOutput<ApiScope>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newApiScope = request.Body.Data;
			var entityApiScope = await GetApiScope(newApiScope.Id);
			if (entityApiScope == null)
			{
				AttachError(response.Header, ResultCode.ApiScopeNotExist, $"ApiScope with data row id: {newApiScope.Id} is not exist.");
				return response;
			}

			var isExist = await configurationDbContext.ApiScopes.AnyAsync(c => c.Id != newApiScope.Id && c.Name == newApiScope.Name);
			if (isExist)
			{
				AttachError(response.Header, ResultCode.ApiScopeExist, "ApiScope name is used by another API scope.");
				return response;
			}

			entityApiScope = newApiScope.UpdateEntity(entityApiScope);

			try
			{
				var entityEntry = configurationDbContext.ApiScopes.Update(entityApiScope);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ViewOutput<ApiScope>
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
		/// Return response object with API scope that deleted by using input parameter。
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
				var entityApiScope = await GetApiScope(request.Body.Id);
				if (entityApiScope != null)
				{
					configurationDbContext.ApiScopes.Remove(entityApiScope);
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

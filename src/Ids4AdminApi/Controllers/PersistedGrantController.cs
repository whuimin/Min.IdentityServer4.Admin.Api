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
	/// Persisted grant operation API。
	/// </summary>
	public class PersistedGrantController : BaseController
	{
		private readonly ILogger<PersistedGrantController> logger;
		private readonly IPersistedGrantDbContext persistedGrantDbContext;

		#region Private method

		/// <summary>
		/// Get persisted grant entity from database.
		/// </summary>
		/// <param name="key">Unique key of data row</param>
		/// <returns></returns>
		private async Task<Entities.PersistedGrant> GetPersistedGrant(string key)
		{
			var query = persistedGrantDbContext.PersistedGrants.Where(c => c.Key == key);

			var entityClient = await query.FirstOrDefaultAsync();
			return entityClient;
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="persistedGrantDbContext"></param>
		public PersistedGrantController(ILogger<PersistedGrantController> logger, IPersistedGrantDbContext persistedGrantDbContext)
		{
			this.logger = logger;
			this.persistedGrantDbContext = persistedGrantDbContext;
		}

		/// <summary>
		/// Return response object with persisted grant list that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<PagingListOutput<PersistedGrant>> List([FromBody] Request<PagingListInput<PersistedGrantListInput>> request)
		{
			var response = new Response<PagingListOutput<PersistedGrant>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var condition = request.Body.Condition;
			var query = persistedGrantDbContext.PersistedGrants.AsQueryable();
			if (!string.IsNullOrEmpty(condition.Key))
			{
				query = query.Where(c => c.Key == condition.Key);
			}

			if (!string.IsNullOrEmpty(condition.Type))
			{
				query = query.Where(c => c.Type == condition.Type);
			}

			if (!string.IsNullOrEmpty(condition.SubjectId))
			{
				query = query.Where(c => c.SubjectId == condition.SubjectId);
			}

			if (!string.IsNullOrEmpty(condition.SessionId))
			{
				query = query.Where(c => c.SessionId == condition.SessionId);
			}

			if (!string.IsNullOrEmpty(condition.ClientId))
			{
				query = query.Where(c => c.ClientId == condition.ClientId);
			}

			if (!string.IsNullOrEmpty(condition.Description))
			{
				query = query.Where(c => c.Description.Contains(condition.Description));
			}

			if (!string.IsNullOrEmpty(condition.Data))
			{
				query = query.Where(c => c.Data.Contains(condition.Data));
			}

			if (condition.CreationTimeBegin.HasValue)
			{
				query = query.Where(c => c.CreationTime >= condition.CreationTimeBegin);
			}
			if (condition.CreationTimeEnd.HasValue)
			{
				query = query.Where(c => c.CreationTime < condition.CreationTimeEnd);
			}

			if (condition.ExpirationBegin.HasValue)
			{
				query = query.Where(c => c.Expiration >= condition.ExpirationBegin);
			}
			if (condition.ExpirationEnd.HasValue)
			{
				query = query.Where(c => c.Expiration < condition.ExpirationEnd);
			}

			if (condition.ConsumedTimeBegin.HasValue)
			{
				query = query.Where(c => c.ConsumedTime >= condition.ConsumedTimeBegin);
			}
			if (condition.ConsumedTimeEnd.HasValue)
			{
				query = query.Where(c => c.ConsumedTime < condition.ConsumedTimeEnd);
			}

			try
			{
				var pageNo = request.Body.PageNo;
				var pageSize = request.Body.PageSize;

				var skip = (pageNo - 1) * pageSize;
				var entityPersistedGrants = query.OrderBy(c => c.Key).Skip(skip).Take(pageSize);
				response.Body = new PagingListOutput<PersistedGrant>
				{
					PageNo = pageNo,
					PageSize = pageSize,
					TotalCount = query.Count(),
					DataList = entityPersistedGrants.ToModels()
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
		/// Return response object with persisted grant that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<PersistedGrant>>> View([FromBody] Request<UniqueKeyInput> request)
		{
			var response = new Response<ViewOutput<PersistedGrant>>
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
				var entityPersistedGrant = await GetPersistedGrant(request.Body.Key);
				if (entityPersistedGrant == null)
				{
					AttachError(response.Header, ResultCode.PersistedGrantNotExist, $"PersistedGrant with data row key: {request.Body.Key} is not exist.");
					return response;
				}

				response.Body = new ViewOutput<PersistedGrant>
				{
					Data = entityPersistedGrant.ToModel()
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
		/// Return response object with persisted grant that deleted by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response> Delete([FromBody] Request<UniqueKeyInput> request)
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
				var entityPersistedGrant = await GetPersistedGrant(request.Body.Key);
				if (entityPersistedGrant != null)
				{
					persistedGrantDbContext.PersistedGrants.Remove(entityPersistedGrant);
					await (persistedGrantDbContext as DbContext).SaveChangesAsync();
				}
			}
			catch(Exception ex)
			{
				AttachError(response.Header, ResultCode.DbError, "Database error.");
				logger.LogError(ex, "Database error.");
			}

			return response;
		}
	}
}

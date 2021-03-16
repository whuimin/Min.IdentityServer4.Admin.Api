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
	/// Device flow code operation API。
	/// </summary>
	public class DeviceFlowCodeController : BaseController
	{
		private readonly ILogger<DeviceFlowCodeController> logger;
		private readonly IPersistedGrantDbContext persistedGrantDbContext;

		#region Private method

		/// <summary>
		/// Get device flow code entity from database.
		/// </summary>
		/// <param name="userCode">Unique key of data row</param>
		/// <returns></returns>
		private async Task<Entities.DeviceFlowCodes> GetDeviceFlowCode(string userCode)
		{
			var query = persistedGrantDbContext.DeviceFlowCodes.Where(c => c.UserCode == userCode);

			var entityClient = await query.FirstOrDefaultAsync();
			return entityClient;
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="persistedGrantDbContext"></param>
		public DeviceFlowCodeController(ILogger<DeviceFlowCodeController> logger, IPersistedGrantDbContext persistedGrantDbContext)
		{
			this.logger = logger;
			this.persistedGrantDbContext = persistedGrantDbContext;
		}

		/// <summary>
		/// Return response object with device flow code list that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<PagingListOutput<DeviceFlowCode>> List([FromBody] Request<PagingListInput<DeviceFlowCodeListInput>> request)
		{
			var response = new Response<PagingListOutput<DeviceFlowCode>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var condition = request.Body.Condition;
			var query = persistedGrantDbContext.DeviceFlowCodes.AsQueryable();
			if (!string.IsNullOrEmpty(condition.UserCode))
			{
				query = query.Where(c => c.UserCode == condition.UserCode);
			}

			if (!string.IsNullOrEmpty(condition.DeviceCode))
			{
				query = query.Where(c => c.DeviceCode == condition.DeviceCode);
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

			try
			{
				var pageNo = request.Body.PageNo;
				var pageSize = request.Body.PageSize;

				var skip = (pageNo - 1) * pageSize;
				var entityDeviceFlowCodes = query.OrderBy(c => c.UserCode).Skip(skip).Take(pageSize);
				response.Body = new PagingListOutput<DeviceFlowCode>
				{
					PageNo = pageNo,
					PageSize = pageSize,
					TotalCount = query.Count(),
					DataList = entityDeviceFlowCodes.ToModels()
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
		/// Return response object with device flow code that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ViewOutput<DeviceFlowCode>>> View([FromBody] Request<UniqueKeyInput> request)
		{
			var response = new Response<ViewOutput<DeviceFlowCode>>
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
				var entityDeviceFlowCode = await GetDeviceFlowCode(request.Body.Key);
				if (entityDeviceFlowCode == null)
				{
					AttachError(response.Header, ResultCode.DeviceFlowCodeNotExist, $"DeviceFlowCode with data row key: {request.Body.Key} is not exist.");
					return response;
				}

				response.Body = new ViewOutput<DeviceFlowCode>
				{
					Data = entityDeviceFlowCode.ToModel()
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
		/// Return response object with device flow code that deleted by using input parameter。
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
				var entityDeviceFlowCode = await GetDeviceFlowCode(request.Body.Key);
				if (entityDeviceFlowCode != null)
				{
					persistedGrantDbContext.DeviceFlowCodes.Remove(entityDeviceFlowCode);
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

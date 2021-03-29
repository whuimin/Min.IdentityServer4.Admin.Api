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
	/// Client operation API。
	/// </summary>
	public class ClientController : BaseController
	{
		private readonly ILogger<ClientController> logger;
		private readonly IConfigurationDbContext configurationDbContext;

		#region Private method

		/// <summary>
		/// Get client entity from database.
		/// </summary>
		/// <param name="id">Unique ID of data row</param>
		/// <returns></returns>
		private async Task<Entities.Client> GetClient(int id)
		{
			var query = configurationDbContext.Clients.Where(c => c.Id == id);

			await query.Include(x => x.AllowedCorsOrigins).SelectMany(c => c.AllowedCorsOrigins).LoadAsync();
			await query.Include(x => x.AllowedGrantTypes).SelectMany(c => c.AllowedGrantTypes).LoadAsync();
			await query.Include(x => x.AllowedScopes).SelectMany(c => c.AllowedScopes).LoadAsync();
			await query.Include(x => x.Claims).SelectMany(c => c.Claims).LoadAsync();
			await query.Include(x => x.ClientSecrets).SelectMany(c => c.ClientSecrets).LoadAsync();
			await query.Include(x => x.IdentityProviderRestrictions).SelectMany(c => c.IdentityProviderRestrictions).LoadAsync();
			await query.Include(x => x.PostLogoutRedirectUris).SelectMany(c => c.PostLogoutRedirectUris).LoadAsync();
			await query.Include(x => x.Properties).SelectMany(c => c.Properties).LoadAsync();
			await query.Include(x => x.RedirectUris).SelectMany(c => c.RedirectUris).LoadAsync();

			var entityClient = await query.FirstOrDefaultAsync();
			return entityClient;
		}

		/// <summary>
		/// Get all available scopes from database.
		/// </summary>
		/// <returns></returns>
		private HashSet<string> GetAllScopes()
		{
			var query = configurationDbContext.ApiScopes.Select(a => a.Name);
			var allScopes = query.ToHashSet();
			var query1 = configurationDbContext.IdentityResources.Select(ir => ir.Name);
			foreach (var scope in query1)
			{
				allScopes.Add(scope);
			}

			return allScopes;
		}

		#endregion

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="configurationDbContext"></param>
		public ClientController(ILogger<ClientController> logger, IConfigurationDbContext configurationDbContext)
		{
			this.logger = logger;
			this.configurationDbContext = configurationDbContext;
		}

		/// <summary>
		/// Return response object with client list that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<PagingListOutput<Client>> List([FromBody] Request<PagingListInput<ClientListInput>> request)
		{
			var response = new Response<PagingListOutput<Client>>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var condition = request.Body.Condition;
			var query = configurationDbContext.Clients.AsQueryable();
			if (condition.Enabled.HasValue)
			{
				query = query.Where(c => c.Enabled == condition.Enabled);
			}

			if (!string.IsNullOrEmpty(condition.ClientId))
			{
				query = query.Where(c => c.ClientId == condition.ClientId);
			}

			if (!string.IsNullOrEmpty(condition.ClientName))
			{
				query = query.Where(c => c.ClientName.Contains(condition.ClientName));
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
				var entityClients = query.OrderBy(c => c.Id).Skip(skip).Take(pageSize);
				response.Body = new PagingListOutput<Client>
				{
					PageNo = pageNo,
					PageSize = pageSize,
					TotalCount = query.Count(),
					DataList = entityClients.ToModels()
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
		/// Return response object with client that filtered by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ClientViewOutput>> View([FromBody] Request<UniqueIdInput> request)
		{
			var response = new Response<ClientViewOutput>
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
				var entityClient = await GetClient(request.Body.Id);
				if (entityClient == null)
				{
					AttachError(response.Header, ResultCode.ClientNotExist, $"Client with data row id: {request.Body.Id} is not exist.");
					return response;
				}

				response.Body = new ClientViewOutput
				{
					Data = entityClient.ToModel(),
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
		/// Return response object with client that initialized。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public Response<ClientViewOutput> Init([FromBody] Request request)
		{
			var response = new Response<ClientViewOutput>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var client = new Client
			{
				ClientId = "NewClientId"
			};
			client.ClientSecrets.Add(new Secret(Guid.NewGuid().ToString("N").Substring(0, 8)));

			response.Body = new ClientViewOutput
			{
				Data = client,
				AllScopes = GetAllScopes()
			};
			return response;
		}

		/// <summary>
		/// Return response object with client that inserted by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ClientViewOutput>> Insert([FromBody] Request<InsertUpdateInput<Client>> request)
		{
			var response = new Response<ClientViewOutput>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newClient = request.Body.Data;
			newClient.Created = DateTime.UtcNow;
			newClient.Updated = null;
			newClient.LastAccessed = null;
			foreach (var secret in newClient.ClientSecrets)
			{
				secret.Value = secret.Value.Sha512();
				secret.Created = DateTime.UtcNow;
			}

			var entityClient = await configurationDbContext.Clients.FirstOrDefaultAsync(c => c.ClientId == newClient.ClientId);
			if (entityClient != null)
			{
				AttachError(response.Header, ResultCode.ClientExist, "Client ID is used by another client.");
				return response;
			}

			entityClient = newClient.ToEntity();

			try
			{
				var entityEntry = await configurationDbContext.Clients.AddAsync(entityClient);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ClientViewOutput
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
		/// Return response object with client that updated by using input parameter。
		/// </summary>
		/// <param name="request">The request parameter</param>
		/// <returns>Return response object</returns>
		[HttpPost]
		public async Task<Response<ClientViewOutput>> Update([FromBody] Request<InsertUpdateInput<Client>> request)
		{
			var response = new Response<ClientViewOutput>
			{
				Header = new ResponseHeader
				{
					Version = request.Header.Version,
					ResponseId = request.Header.RequestId,
					IsSuccess = true
				}
			};

			var newClient = request.Body.Data;
			var entityClient = await GetClient(newClient.Id);
			if (entityClient == null)
			{
				AttachError(response.Header, ResultCode.ClientNotExist, $"Client with data row id: {newClient.Id} is not exist.");
				return response;
			}

			var isExist = await configurationDbContext.Clients.AnyAsync(c => c.Id != newClient.Id && c.ClientId == newClient.ClientId);
			if (isExist)
			{
				AttachError(response.Header, ResultCode.ClientExist, "Client ID is used by another client.");
				return response;
			}

			newClient.Created = entityClient.Created;
			newClient.Updated = DateTime.UtcNow;
			newClient.LastAccessed = entityClient.LastAccessed;
			var entityClientMapping = entityClient.ClientSecrets?.ToDictionary(cs => cs.Id) ?? new Dictionary<int, Entities.ClientSecret>();
			foreach (var secret in newClient.ClientSecrets)
			{
				var entitySecret = entityClientMapping.GetValueOrDefault(secret.Id);
				if (entitySecret == null || entitySecret.Value != secret.Value)
				{
					secret.Value = secret.Value.Sha512();
					secret.Created = DateTime.UtcNow;
				}
			}

			entityClient = newClient.UpdateEntity(entityClient);

			try
			{
				var entityEntry = configurationDbContext.Clients.Update(entityClient);
				await (configurationDbContext as DbContext).SaveChangesAsync();
				response.Body = new ClientViewOutput
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
		/// Return response object with client that deleted by using input parameter。
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
				var entityClient = await GetClient(request.Body.Id);
				if (entityClient != null)
				{
					configurationDbContext.Clients.Remove(entityClient);
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

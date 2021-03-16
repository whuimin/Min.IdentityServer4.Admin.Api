using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ids4AdminApi.Models;

namespace Ids4AdminApi.Controllers
{
	/// <summary>
	/// API base controller.
	/// </summary>
	[ApiController]
	[Route("api/[controller]/[action]")]
	public class BaseController : ControllerBase
	{
		/// <summary>
		/// Attach error to response header.
		/// </summary>
		/// <param name="responseHeader">Response header object</param>
		/// <param name="resultCode">Result code</param>
		/// <param name="message">Error message</param>
		/// <returns></returns>
		protected void AttachError(ResponseHeader responseHeader, ResultCode resultCode, string message)
		{
			var errors= new List<ResponseError>
			{
				new ResponseError() { Code = resultCode.ToString(), Message = message }
			};

			responseHeader.IsSuccess = false;
			responseHeader.Errors = errors;
		}
	}
}

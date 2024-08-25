using Laymaann.Entities.Shared;
using Laymaann.Entities.ViewModels.Blog;
using Laymaann.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Security.Claims;

namespace Laymaann.Web.Controllers.Api
{
	[Authorize(Roles ="3")]
	[Route("api/admin")]
	[ApiController]
	public class AdminController : FoundationController
	{
		public readonly IUserRepository _userRepo;

		public AdminController(IOptionsMonitor<LaymaannConfig> config, ILogger<FoundationController> logger, IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) : base(config, logger, httpContextAccessor)
		{
			_userRepo = userRepository;
		}

		[HttpGet("users")]
		#region Get All Users
		public async Task<IActionResult> GetAllUsers()
		{
			int statCode = StatusCodes.Status200OK;
			List<string> errors = [];
			string Message = "retrieving claims";
			List<AcUser> users = [];

			return await ExecuteActionAsync(async () =>
			{

				//var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

				//if (roleClaim != null && roleClaim.Value == "3")
				//{
				//	var role = roleClaim.Value;
					users = await _userRepo.GetAllUsers();
				//}

				return (statCode, users, Message, errors);

			}, MethodBase.GetCurrentMethod().Name);
		}
		#endregion


		[HttpGet("current-claims")]
		#region Get All Claims
		public async Task<IActionResult> GetAllBlogs()
		{
			int statCode = StatusCodes.Status200OK;
			List<string> errors = [];
			string Message = "retrieving claims";

			return await ExecuteActionAsync(async () =>
			{
				var claims = User.Claims.Select(c => new
				{
					c.Type,
					c.Value
				});
				return (statCode, claims, Message, errors);

			}, MethodBase.GetCurrentMethod().Name);
		}
		#endregion

		public static string GetUserRole(ClaimsPrincipal user)
		{
			var roleClaim = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);

			if (roleClaim != null)
			{
				return roleClaim.Value;
			}

			return null; // Role not found
		}

	}
}

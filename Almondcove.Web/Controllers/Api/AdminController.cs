using Laymaann.Entities.Shared;
using Laymaann.Entities.ViewModels.Blog;
using Laymaann.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Laymaann.Web.Controllers.Api
{
	[Authorize]
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

            return await ExecuteActionAsync(async () =>
            {
                var users = await _userRepo.GetAllUsers();
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

	}
}

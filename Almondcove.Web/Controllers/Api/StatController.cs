using Laymaann.Entities.Shared;
using Laymaann.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Laymaann.Web.Controllers.Api
{
    [Route("api/stat")]
    [ApiController]
    public class StatController : FoundationController
    {
        private readonly IUserRepository _userRepo;
        public StatController(IOptionsMonitor<LaymaannConfig> config, ILogger<FoundationController> logger,IHttpContextAccessor httpContextAccessor, IUserRepository userRepository) 
            : base(config, logger, httpContextAccessor)
        {
            _userRepo = userRepository;
        }

        [HttpGet("usercount")]
        #region COUNT USER
        public async Task<IActionResult> CountrUser()
        {
            int statCode = StatusCodes.Status400BadRequest;
            List<string> errors = [];
            string Message = "";
            int count = 0;

            statCode = StatusCodes.Status200OK;

            return await ExecuteActionAsync(async () =>
            {
                if (errors.Count == 0)
                {
                    count = await _userRepo.GetUserCountAsync();
                    Message = "Count loaded";
                    statCode = StatusCodes.Status200OK;
                }
                
                return (statCode, count, Message, errors);

            }, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

    }
}

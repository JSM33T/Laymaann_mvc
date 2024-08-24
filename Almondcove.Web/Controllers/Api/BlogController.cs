using Laymaann.Entities.Shared;
using Laymaann.Entities.ViewModels.Blog;
using Laymaann.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Laymaann.Web.Controllers.Api
{
    [Route("api/blogs")]
    [ApiController]
    public class BlogController : FoundationController
    {
        private readonly IBlogRepository _blogRepo;
        public BlogController(IOptionsMonitor<LaymaannConfig> config, ILogger<FoundationController> logger,IHttpContextAccessor httpContextAccessor,IBlogRepository blogRepository) 
            : base(config, logger, httpContextAccessor)
        {
            _blogRepo = blogRepository;
        }

        [HttpGet("getall")]
        #region blogs CONTROLLER
        public async Task<IActionResult> GetAllBlogs()
        {
            return await ExecuteActionAsync(async () =>
            {
                int statCode = StatusCodes.Status400BadRequest;
                List<BlogPost> blogPosts = [];
                List<string> errors = [];


                statCode = StatusCodes.Status200OK;
                blogPosts = await _blogRepo.GetAllBlogsAsync();
                return (statCode, blogPosts, "retrieving blogs", errors);

            }, MethodBase.GetCurrentMethod().Name);
        }
        #endregion

    }
}

using Laymaann.Repositories;
using Markdig;
using Microsoft.AspNetCore.Mvc;

namespace Laymaann.Web.Controllers.Routes
{
	
	public class BlogRouteController : Controller
	{
		private readonly IBlogRepository _blogRepo;
        public BlogRouteController(IBlogRepository blogRepository)
        {
			_blogRepo = blogRepository;
        }
        [Route("blogs")]
		public IActionResult Index()
		{
			return View("Views/Blogs/Index.cshtml");
		}

		[Route("blogs/browse")]
		public IActionResult Browse()
		{
			return View("Views/Blogs/Browse.cshtml");
		}

		[Route("blog/{Year}/{Slug}")]
		public async Task<IActionResult> View(string Year,string Slug)
		{
			var blogPost = await _blogRepo.GetBlogPostByYearAndSlugAsync(Year, Slug);

			if (blogPost == null)
			{
				//eturn NotFound();
                return View("Views/ErrorPages/NotFound.cshtml");
            }

			var pipeline = new MarkdownPipelineBuilder().Build();

			blogPost.Content = Markdown.ToHtml(blogPost.Content, pipeline);
			return View("Views/Blogs/Viewer.cshtml", blogPost);
		}

	}
}

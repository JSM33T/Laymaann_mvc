using Laymaann.Entities.ViewModels.Blog;

namespace Laymaann.Repositories
{
    public interface IBlogRepository
	{
		public Task<BlogPost> GetBlogPostByYearAndSlugAsync(string year, string slug);
		public Task<List<BlogPost>> GetAllBlogsAsync();

    }
}

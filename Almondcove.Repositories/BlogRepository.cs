using Laymaann.Entities.Shared;
using Laymaann.Entities.ViewModels.Blog;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laymaann.Repositories
{
	public class BlogRepository : IBlogRepository
	{
		protected readonly IOptionsMonitor<LaymaannConfig> _config;
		protected readonly ILogger<IBlogRepository> _logger;
		private string _conStr;
		public BlogRepository(IOptionsMonitor<LaymaannConfig> config, ILogger<IBlogRepository> logger)
		{
			_config = config;
			_logger = logger;
			_conStr = _config.CurrentValue.ConnectionString;
		}
		public async Task<BlogPost> GetBlogPostByYearAndSlugAsync(string year, string slug)
		{
			using (var connection = new SqlConnection(_conStr))
			{
				var sql = "usp_GetBlogPostByYearAndSlug";

				return await connection.QuerySingleOrDefaultAsync<BlogPost>(
					sql,
					new { Year = year, Slug = slug },
					commandType: System.Data.CommandType.StoredProcedure
				);
			}
		}

        public async Task<List<BlogPost>> GetAllBlogsAsync()
        {
            using (var connection = new SqlConnection(_conStr))
            {
                var sql = "usp_GetAllBlogPosts";

                var result = await connection.QueryAsync<BlogPost>(
                    sql,
                    commandType: System.Data.CommandType.StoredProcedure
                );

                return result.AsList();
            }
        }

    }

}

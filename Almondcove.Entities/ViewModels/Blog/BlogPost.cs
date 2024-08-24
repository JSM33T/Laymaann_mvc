using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laymaann.Entities.ViewModels.Blog
{

	public class BlogPost
	{
		public int Id { get; set; }
		public string Title { get; set; }
		public string Slug { get; set; }
		public string Description { get; set; }
		public string Tags { get; set; }
		public string Content { get; set; }
		public int BlogSeriesId { get; set; }
		public int BlogCategory { get; set; }
		public bool IsActive { get; set; }
		public DateTime DateAdded { get; set; }
	}
}

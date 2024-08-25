using Laymaann.Entities.ViewModels.Base;
using Laymaann.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Laymaann.Web.Controllers.Routes
{
    public class BaseController(ILogger<BaseController> logger, IWebHostEnvironment webHostEnvironment) : Controller
    {
        private readonly ILogger<BaseController> _logger = logger;
		private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

		[Route("/")]
		public IActionResult Index() => View("Views/Base/Index.cshtml");

		[Route("/about")]
		public IActionResult About() => View("Views/Base/About.cshtml");

		[Route("/changelog")]
		public IActionResult Changelog()
		{
			return View("Views/Base/Changelog.cshtml");
		} 

		[Route("/attributions")]
		public IActionResult Attributions() => View("Views/Base/Attributions.cshtml");

		[Route("/faq")]
		public async Task<IActionResult> FAQs()
		{
			var filePath = Path.Combine(_webHostEnvironment.WebRootPath,"data", "faqs.json");
			List<FaqItem> faqs;

			if (System.IO.File.Exists(filePath))
			{
				var json = await System.IO.File.ReadAllTextAsync(filePath);
				faqs = JsonConvert.DeserializeObject<List<FaqItem>>(json);
			}
			else
			{
				faqs = new List<FaqItem>();
			}

			return View("Views/Base/Faq.cshtml", faqs);
		}
		

		[Route("/contact")]
		public IActionResult Contact() => View("Views/Base/Contact.cshtml");

		[Route("/accessdenied")]
		public IActionResult AccessDenied() => View("Views/ErrorPages/AccessDenied.cshtml");

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

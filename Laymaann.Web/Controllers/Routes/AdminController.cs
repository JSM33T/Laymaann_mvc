using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Laymaann.Web.Controllers.Routes
{
    [Authorize(Roles = "3")]
    public class AdminController : Controller
    {
        [Route("/admin")]
        public IActionResult Index()
        {
            return View("Views/Admin/Dashboard.cshtml");
        }
    }
}

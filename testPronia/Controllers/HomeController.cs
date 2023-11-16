using Microsoft.AspNetCore.Mvc;

namespace testPronia.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

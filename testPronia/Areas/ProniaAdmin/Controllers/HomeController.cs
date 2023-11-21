using Microsoft.AspNetCore.Mvc;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

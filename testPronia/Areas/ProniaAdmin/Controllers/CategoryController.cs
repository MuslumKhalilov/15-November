using Microsoft.AspNetCore.Mvc;

namespace testPronia.Areas.ProniaAdmin.Controllers
{
    [Area("ProniaAdmin")]
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

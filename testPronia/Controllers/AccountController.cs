using Microsoft.AspNetCore.Mvc;
using testPronia.ModelViews;

namespace testPronia.Controllers
{
	public class AccountController : Controller
	{
		public IActionResult Register()
		{
			
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterVM registerVM)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}
			return Json(registerVM);
		}
	}
}

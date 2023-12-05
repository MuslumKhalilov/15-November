﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using testPronia.Models;
using testPronia.ModelViews;

namespace testPronia.Controllers
{
	public class AccountController : Controller
	{
		private readonly UserManager<AppUser> _userManagaer;
		private readonly SignInManager<AppUser> _signInManager;

		public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager)
        {
				_userManagaer=userManager;
			_signInManager= signInManager;
        }
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
			AppUser user = new AppUser()
			{
				Name = registerVM.Name,
				Surname = registerVM.Surname,
				UserName = registerVM.Username,
				Email = registerVM.Email
			};

			IdentityResult result= await _userManagaer.CreateAsync(user,registerVM.Password);
			if (!result.Succeeded)
			{
				foreach (IdentityError error in result.Errors)
				{
					ModelState.AddModelError(String.Empty,error.Description);
				}
				return View();
			}
			await _signInManager.SignInAsync(user,false);
			return RedirectToAction("Index", "Home");
		}
	}
}
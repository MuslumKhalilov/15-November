﻿using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using testPronia.Models;
using testPronia.ModelViews;

namespace testPronia.Controllers;

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
            string regex = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex regex1 = new Regex(regex);
            if (!regex1.IsMatch(registerVM.Email))
            {
			ModelState.AddModelError("Email","Email dogru sekilde deyil");   
		}
		
            

            AppUser user = new AppUser()
		{
			Name = registerVM.Name,
			Surname = registerVM.Surname,
			UserName = registerVM.Username,
			Email = registerVM.Email,
			//Gender = registerVM.Gender
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
	public async Task<IActionResult> LogOut()
	{
		_signInManager.SignOutAsync();
		return RedirectToAction("Index", "Home");
	}
	public IActionResult Login()
	{
		return View();
	}
	[HttpPost]
    public async Task<IActionResult> Login(LoginVM loginVM, string? returnUrl)
    {
		if (!ModelState.IsValid)
		{
            return View();
        }
		AppUser user = await _userManagaer.FindByNameAsync(loginVM.UserNameOrEmail);
		if (user == null)
		{
			user = await _userManagaer.FindByEmailAsync(loginVM.UserNameOrEmail);
			if(user == null)
			{
				ModelState.AddModelError(String.Empty,"UserName, Email or Password is incorrect");
				return View();
			}
		}
		var result=await _signInManager.PasswordSignInAsync(user,loginVM.Password,loginVM.IsRemembered,true);
		if (result.IsLockedOut)
		{
			ModelState.AddModelError(String.Empty,"Your account is locked. Please try again later.");
		}
		
		if (!result.Succeeded)
		{
			ModelState.AddModelError(String.Empty, "UserName, Email or Password is incorrect");
			return View();
		}
		if ( returnUrl is null)
		{
			return RedirectToAction("Index", "Home");
		}
		return Redirect(returnUrl);

	}
}

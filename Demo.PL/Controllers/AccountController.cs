using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Evaluation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Reflection.Metadata;
using System.Security.Principal;

namespace Demo.PL.Controllers
{

    public class AccountController : Controller
	{
		private readonly UserManager<ApplicationUser> userManager;
		private readonly SignInManager<ApplicationUser> signInManager;
		private readonly ILogger<AccountController> logger;

		public AccountController
			(UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager,
			ILogger<AccountController> logger)
        {
			this.userManager = userManager;
			this.signInManager = signInManager;
			this.logger = logger;
		}
        public IActionResult SignUp () 
		{ 
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignUp (SignUpViewModel input)
		{
			var users = await userManager.Users.ToListAsync();
			foreach(var user in users)
				if (input.Email == user.Email)
					ModelState.AddModelError("", "This Email Already Exist");

			if (ModelState.IsValid)
			{
				var user = new ApplicationUser
				{
					Email = input.Email,
					UserName = input.Email.Split("@")[0],
					IsActive = true
				};
				var result = await userManager.CreateAsync(user, input.Password);

				if (result.Succeeded)
					return RedirectToAction("Login");

				foreach(var error in result.Errors)
				{
					logger.LogError(error.Description);
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(input);
		}

		public IActionResult Login ()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login (SignInViewModel input)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(input.Email);
				if (user == null)
					ModelState.AddModelError("", "Email Not Found");
				if (user is not null && await userManager.CheckPasswordAsync(user, input.Password))
				{
					var result = await signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, true);
					if (result.Succeeded)
						return RedirectToAction("Index", "Home");
				}
			}
			return View(input);
		}
		public async Task<IActionResult> SignOut ()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Login");
		}
		public IActionResult ForgetPassword()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel input)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync (input.Email);
				if (user is null)
					ModelState.AddModelError("", "Email Dosen't Exist ");
				if(user is not null)
				{
					var token = await userManager.GeneratePasswordResetTokenAsync(user);
					var resetPasswordLink = Url.Action("ResetPassword", "Account",
						new { Email = input.Email, Token = token }, Request.Scheme);
					var email = new Email
					{
						Title = "Reset Password",
						Body = resetPasswordLink,
						To = input.Email
					};
					EmailSettings.SendEmail(email);
					return RedirectToAction("CompleteForgetPassword");
				}
			}
			return View(input);
		}
		public IActionResult ResetPassword(string email, string token)
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel input)
		{
			if (ModelState.IsValid)
			{
				var user = await userManager.FindByEmailAsync(input.Email);
				if (user is null)
					ModelState.AddModelError("", "Email Dosen't Exist");
				if (user is not null)
				{
					var result = await userManager.ResetPasswordAsync(user, input.Token, input.Password);
					if (result.Succeeded)
						return RedirectToAction("Login");
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
						logger.LogError(error.Description);
					}
				}
			}
			return View(input);
		}
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}

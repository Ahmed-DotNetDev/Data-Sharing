using FileSharingAPP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileSharingAPP.Controllers
{
	public class AccountController : Controller
	{

		private readonly SignInManager<IdentityUser> signInManager;
		private readonly UserManager<IdentityUser> userManager;

		public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
		{

			this.signInManager = signInManager;
			this.userManager = userManager;
		}
		public IActionResult Index()
		{
			return View();
		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel model)
		{
			if (ModelState.IsValid)
			{
				var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, true);
				if (result.Succeeded)
				{
					return RedirectToAction("Create", "Uploads");
				}
			}
			return View(model);
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = new IdentityUser
				{
					UserName = model.Email,
					Email = model.Email,
				};
				var result = await userManager.CreateAsync(user, model.Password);
				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, true);
					return RedirectToAction("Create", "Uploads");
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}
			return View(model);
		}
		public async Task<IActionResult> Logout()
		{
			await signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult ExternalLogin(string provider)
		{
			//FacebookDefaults.AuthenticationScheme get "Facebook"
			var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, "/Account/ExternalResponse");
			return Challenge(properties, provider);
		}

		public async Task<IActionResult> ExternalResponse()
		{
			var info = await signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				ViewData["Message"] = "Login failed";
				return RedirectToAction("Login");
			}
			var LoginResult = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
			if (!LoginResult.Succeeded)
			{
				//Create account if not login here
				var email = info.Principal.FindFirstValue(ClaimTypes.Email);
				var firstname = info.Principal.FindFirstValue(ClaimTypes.GivenName);
				var lastname = info.Principal.FindFirstValue(ClaimTypes.Surname);
				var UserToCreate = new IdentityUser
				{
					Email = email,
					UserName = email,
				};
				var CreateResult = await userManager.CreateAsync(UserToCreate);
				if (CreateResult.Succeeded)
				{
					var ExLoginResult = await userManager.AddLoginAsync(UserToCreate, info);
					if (ExLoginResult.Succeeded)
					{
						await signInManager.SignInAsync(UserToCreate, isPersistent: false, info.LoginProvider);//ASPNetLogin Tables
						return RedirectToAction("Index", "Home");
					}
					else
					{
						await userManager.DeleteAsync(UserToCreate);
					}
				}
				return RedirectToAction("Login");
			}
			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public IActionResult Info()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var currentUser = await userManager.GetUserAsync(User);
				if (currentUser != null)
				{
					var result = await userManager.ChangePasswordAsync(currentUser, model.NewPassword, model.ConfirmNewPassword);
					if (result.Succeeded)
					{
						return RedirectToAction("Info");
					}
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError("", error.Description);
					}
				}
				else
				{
					return NotFound();
				}
			}
			return RedirectToAction("Info");
		}
		[HttpGet]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		//public async Task<IActionResult> ForgotPassword(ForgotPasswordViewMOdel model)
		//{
		//	if (ModelState.IsValid)
		//	{
		//		var ExistUser = await userManager.FindByEmailAsync(model.Email);
		//		if (ExistUser != null)
		//		{
		//			var Token = await userManager.GeneratePasswordResetTokenAsync(ExistUser);
		//			var URL = Url.Action("ResetPassword", "Account", new { Token, model.Email }, Request.Scheme);
		//			StringBuilder body = new StringBuilder();
		//			body.AppendLine("File sharing app : reset password");
		//			body.AppendLine("We are sending this email to reset password");
		//			body.AppendFormat("to reset new password <a href='{0}'>Click this link</a>", URL);
		//			mailHelper.SendMail(new InputEmailMessage
		//			{
		//				Email = model.Email,
		//				Subject = "Reset password",
		//				Body = body.ToString()
		//			});
		//		}
		//		TempData["Success"] = "if you email match in out system this will recieve to you";
		//	}
		//	return View(model);
		//}
	}

}
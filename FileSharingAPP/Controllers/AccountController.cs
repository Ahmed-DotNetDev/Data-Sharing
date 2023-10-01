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
					UserName = email
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
	}

}
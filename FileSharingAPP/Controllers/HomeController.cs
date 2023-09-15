using FileSharingAPP.Data;
using FileSharingAPP.Helpers.Mail;
using FileSharingAPP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileSharingAPP.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ApplicationDbContext _db;
		private readonly ImailHelper _mailHelper;

		public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, ImailHelper mailHelper)
		{
			_logger = logger;
			this._db = db;
			this._mailHelper = mailHelper;
		}
		private string UserId
		{
			get
			{
				return User.FindFirstValue(ClaimTypes.NameIdentifier);
			}
		}
		public IActionResult Index()
		{
			var HightestDownload = _db.uploads.OrderByDescending(p => p.DownloadCount)
				.Select(u => new UploadViewModel
				{
					Id = u.Id,
					FileName = u.FileName,
					OriginalName = u.OriginalName,
					ContentType = u.ContentType,
					Size = u.Size,
					UploadDate = u.UploadDate,
					DownloadCount = u.DownloadCount,
				})
				.Take(3);
			ViewBag.Popular = HightestDownload;
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
		[HttpGet]
		public IActionResult Info()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		[HttpGet]
		public IActionResult Contact()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> Contact(ContactViewModel model)
		{
			if (ModelState.IsValid)
			{
				//Save
				_db.Contacts.Add(new Data.Contact
				{
					Email = model.Email,
					Message = model.Message,
					Name = model.Name,
					Subject = model.Subject,
					UserId = UserId
				});
				await _db.SaveChangesAsync();
				TempData["Message"] = "Message has been sent successfully!";
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("<h1>Data Sharing App | Unread message</h1>");
				sb.AppendFormat("Name : {0}", model.Name);
				sb.AppendFormat("Email : {0}", model.Email);
				sb.AppendLine();
				sb.AppendFormat("Subject : {0}", model.Subject);
				sb.AppendFormat("Message : {0}", model.Message);

				//Send mail
				_mailHelper.SendMail(new InputEmailMessage
				{
					Subject = "You have unread message",
					Email = "DataSharing@Site.com",
					Body = sb.ToString(),
				});
				return RedirectToAction("Contact");
			}
			return View();
		}
		[HttpGet]
		public IActionResult About()
		{
			return View();
		}
		[HttpGet]
		public IActionResult SetCulture(string Lang)
		{
			if (!string.IsNullOrEmpty(Lang))
			{
				Response.Cookies.Append(
				 CookieRequestCultureProvider.DefaultCookieName,
				 CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Lang)),
		  	new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
			   );
			}
			return RedirectToAction("Index");
		}
	}
}

using FileSharingAPP.Models;
using FileSharingAPP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FileSharingAPP.Controllers
{
	[Authorize]
	public class UploadsController : Controller
	{
		private readonly IUploadService uploadService;
		private readonly IWebHostEnvironment env;

		public UploadsController(IUploadService uploadService, IWebHostEnvironment env)
		{
			this.uploadService = uploadService;
			this.env = env;
		}

		public IActionResult Index()
		{
			var result = uploadService.GetBy(UserId);
			return View(result);
		}
		private string UserId
		{
			get
			{
				return User.FindFirstValue(ClaimTypes.NameIdentifier);
			}
		}
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(InputFile model)
		{
			if (ModelState.IsValid)
			{
				var NewName = Guid.NewGuid().ToString();
				var Extension = Path.GetExtension(model.File.FileName);
				var FileName = string.Concat(NewName, Extension);
				var Root = env.WebRootPath;
				var path = Path.Combine(Root, "Uploads", FileName);

				using (var fs = System.IO.File.Create(path))
				{
					await model.File.CopyToAsync(fs);
				}
				await uploadService.CreateAsync(new InputUpload
				{
					OriginalName = model.File.FileName,
					FileName = FileName,
					ContentType = model.File.ContentType,
					Size = model.File.Length,
					UserId = UserId,
				});

				return RedirectToAction("Index");
			}
			return View(model);
		}
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var SelectedUpload = await uploadService.FindAsync(id);
			if (SelectedUpload == null)
			{
				return NotFound();
			}
			ViewBag.UploadId = id;
			return View(SelectedUpload);
		}

		[ActionName("Delete")]
		[HttpPost]
		public async Task<IActionResult> DeleteConfirmation(string id)
		{

			var SelectedUpload = await uploadService.FindAsync(id);
			if (SelectedUpload == null)
			{
				return NotFound();
			}
			await uploadService.DeleteAsync(id);
			return RedirectToAction("Index");
		}
		[HttpPost]
		[AllowAnonymous]
		public IActionResult Results(string term)
		{
			var model = uploadService.Search(term);

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Browse(int RequiredPage = 1)
		{
			const int PageSize = 3;
			int SkipCount = (RequiredPage - 1) * PageSize;
			decimal RowsCount = await uploadService.GetUploadsCount();
			var PagesCount = Math.Ceiling(RowsCount / PageSize);
			if (RequiredPage > PagesCount)
			{
				RequiredPage = 1;
			}
			RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;

			var model = await uploadService.GetAll().Skip(SkipCount)
				.Take(PageSize).ToListAsync();

			ViewBag.CurrentPage = RequiredPage;
			ViewBag.PagesCount = PagesCount;
			return View(model);
		}
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Downloads(string id)
		{
			var SelectedFile = await uploadService.FindForDownloadAsync(id);
			if (SelectedFile == null)
			{
				return NotFound();
			}
			await uploadService.IncrementDownloadCount(id);
			var Path = "~/Uploads/" + SelectedFile.FileName;
			Response.Headers.Add("Expires", DateTime.Now.AddDays(-3).ToLongDateString());
			Response.Headers.Add("Cache-Control", "no-cache");
			return File(Path, SelectedFile.ContentType, SelectedFile.OriginalName);
		}
	}
}

using FileSharingAPP.Data;
using FileSharingAPP.Models;
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
		private readonly ApplicationDbContext _db;
		private readonly IWebHostEnvironment env;

		public UploadsController(ApplicationDbContext context, IWebHostEnvironment env)
		{
			this._db = context;
			this.env = env;
		}

		public IActionResult Index()
		{
			var result = _db.uploads.Where(u => u.UserId == UserId)
				.OrderByDescending(u => u.DownloadCount)
				.Select(u => new UploadViewModel
				{
					Id = u.Id,
					FileName = u.FileName,
					OriginalName = u.OriginalName,
					ContentType = u.ContentType,
					Size = u.Size,
					UploadDate = u.UploadDate,
					DownloadCount = u.DownloadCount,
				});
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
		public async Task<IActionResult> Create(InputUpload model)
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
				await _db.uploads.AddAsync(new Uploads
				{
					OriginalName = model.File.FileName,
					FileName = FileName,
					ContentType = model.File.ContentType,
					Size = model.File.Length,
					UserId = UserId,
				});
				await _db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			return View(model);
		}
		[HttpGet]
		public async Task<IActionResult> Delete(string id)
		{
			var SelectedUpload = await _db.uploads.FindAsync(id);
			if (SelectedUpload == null)
			{
				return NotFound();
			}
			if (SelectedUpload.UserId != UserId)
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

			var SelectedUpload = await _db.uploads.FindAsync(id);
			if (SelectedUpload == null)
			{
				return NotFound();
			}

			_db.uploads.Remove(SelectedUpload);
			await _db.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		[HttpPost]
		[AllowAnonymous]
		public IActionResult Results(string term)
		{
			var model = _db.uploads.Where(p => p.OriginalName.Contains(term))
				.Select(u => new UploadViewModel
				{
					FileName = u.FileName,
					OriginalName = u.OriginalName,
					ContentType = u.ContentType,
					Size = u.Size,
					UploadDate = u.UploadDate,
					DownloadCount = u.DownloadCount,

				});

			return View(model);
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Browse(int RequiredPage = 1)
		{
			const int PageSize = 3;
			int SkipCount = (RequiredPage - 1) * PageSize;
			decimal RowsCount = await _db.uploads.CountAsync();
			var PagesCount = Math.Ceiling(RowsCount / PageSize);
			if (RequiredPage > PagesCount)
			{
				RequiredPage = 1;
			}
			RequiredPage = RequiredPage <= 0 ? 1 : RequiredPage;
			var model = await _db.uploads
				.OrderByDescending(p => p.DownloadCount)
				.Select(u => new UploadViewModel
				{
					FileName = u.FileName,
					OriginalName = u.OriginalName,
					ContentType = u.ContentType,
					Size = u.Size,
					UploadDate = u.UploadDate,
				})
				.Skip(SkipCount)
				.Take(PageSize).ToListAsync();
			ViewBag.CurrentPage = RequiredPage;
			ViewBag.PagesCount = PagesCount;
			return View(model);
		}
		[HttpGet]
		[AllowAnonymous]
		public async Task<IActionResult> Downloads(string id)
		{
			var SelectedFile = await _db.uploads.FirstOrDefaultAsync(p => p.FileName == id);
			if (SelectedFile == null)
			{
				return NotFound();
			}
			SelectedFile.DownloadCount++;
			_db.Update(SelectedFile);
			await _db.SaveChangesAsync();
			var Path = "~/Uploads/" + SelectedFile.FileName;
			Response.Headers.Add("Expires", DateTime.Now.AddDays(-3).ToLongDateString());
			Response.Headers.Add("Cache-Control", "no-cache");
			return File(Path, SelectedFile.ContentType, SelectedFile.OriginalName);
		}
	}
}

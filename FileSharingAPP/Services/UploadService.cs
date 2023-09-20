using FileSharingAPP.Data;
using FileSharingAPP.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingAPP.Services
{
	public class UploadService : IUploadService
	{
		private readonly ApplicationDbContext _db;

		public UploadService(ApplicationDbContext context)
		{
			this._db = context;
		}
		public async Task CreateAsync(InputUpload model)
		{
			var result = await _db.uploads.AddAsync(new Uploads
			{
				OriginalName = model.OriginalName,
				FileName = model.FileName,
				ContentType = model.ContentType,
				Size = model.Size,
				UserId = model.UserId,
			});
			await _db.SaveChangesAsync();
		}

		public async Task DeleteAsync(string id)
		{
			var SelectedUpload = await _db.uploads.FirstOrDefaultAsync(u => u.Id == id);
			if (SelectedUpload != null)
			{
				_db.uploads.Remove(SelectedUpload);
				await _db.SaveChangesAsync();
			}
		}
		public async Task<Uploads> FindAsync(string id)
		{
			var SelectedUpload = await _db.uploads.FindAsync(id);
			if (SelectedUpload != null)
			{
				return new Uploads
				{
					Id = SelectedUpload.Id,
					FileName = SelectedUpload.FileName,
					OriginalName = SelectedUpload.OriginalName,
					ContentType = SelectedUpload.ContentType,
					Size = SelectedUpload.Size,
					DownloadCount = SelectedUpload.DownloadCount
				};
			}
			return null;
		}

		public async Task<Uploads> FindForDownloadAsync(string id)
		{
			var SelectedUpload = await _db.uploads.FirstOrDefaultAsync(u => u.FileName == id);
			if (SelectedUpload != null)
			{
				return new Uploads
				{
					Id = SelectedUpload.Id,
					FileName = SelectedUpload.FileName,
					OriginalName = SelectedUpload.OriginalName,
					ContentType = SelectedUpload.ContentType,
					Size = SelectedUpload.Size,
					DownloadCount = SelectedUpload.DownloadCount
				};
			}
			return null;
		}

		public IQueryable<UploadViewModel> GetAll()
		{
			var result = _db.uploads
				.OrderByDescending(p => p.DownloadCount)
				.Select(u => new UploadViewModel
				{
					FileName = u.FileName,
					OriginalName = u.OriginalName,
					ContentType = u.ContentType,
					Size = u.Size,
					UploadDate = u.UploadDate,
				});
			return result;
		}

		public IQueryable<UploadViewModel> GetBy(string userId)
		{
			var result = _db.uploads.Where(u => u.UserId == userId)
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
			return result;
		}

		public async Task<int> GetUploadsCount()
		{
			return await _db.uploads.CountAsync();
		}

		public async Task IncrementDownloadCount(string id)
		{
			var SelectedUpload = await _db.uploads.FindAsync(id);
			if (SelectedUpload != null)
			{
				SelectedUpload.DownloadCount++;
				_db.Update(SelectedUpload);
				await _db.SaveChangesAsync();
			}
		}

		public IQueryable<UploadViewModel> Search(string term)
		{
			var result = _db.uploads.Where(p => p.OriginalName.Contains(term))
				.Select(u => new UploadViewModel
				{
					FileName = u.FileName,
					OriginalName = u.OriginalName,
					ContentType = u.ContentType,
					Size = u.Size,
					UploadDate = u.UploadDate,
					DownloadCount = u.DownloadCount,

				});
			return result;
		}
	}
}

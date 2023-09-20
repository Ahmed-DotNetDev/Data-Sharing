using FileSharingAPP.Data;
using FileSharingAPP.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingAPP.Services
{
	public interface IUploadService
	{
		IQueryable<UploadViewModel> GetAll();
		IQueryable<UploadViewModel> GetBy(string userId);
		IQueryable<UploadViewModel> Search(string term);
		Task CreateAsync(InputUpload model);
		Task<Uploads> FindAsync(string id);
		Task<Uploads> FindForDownloadAsync(string id);
		Task DeleteAsync(string id);
		Task IncrementDownloadCount(string id);
		Task<int> GetUploadsCount();
	}
}

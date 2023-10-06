using FileSharingAPP.Areas.Admin.Models;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingAPP.Areas.Admin.Services
{
	public interface IUserService
	{
		IQueryable<AdminUserViewModel> GetAll();
		IQueryable<AdminUserViewModel> GetBlockedUser();
		IQueryable<AdminUserViewModel> Search(string term);
		Task<OperationResult> BlockUser(string userid);
		Task<int> UserRegistrationCount(int month);
	}
}

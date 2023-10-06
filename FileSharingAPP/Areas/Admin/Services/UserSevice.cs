using FileSharingAPP.Areas.Admin.Models;
using FileSharingAPP.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingAPP.Areas.Admin.Services
{
	public class UserSevice : IUserService
	{
		private readonly ApplicationDbContext context;

		public UserSevice(ApplicationDbContext context)
		{
			this.context = context;
		}
		public async Task<OperationResult> BlockUser(string userid)
		{
			var ExsistedUser = await context.Users.FindAsync(userid);
			if (ExsistedUser == null)
			{
				return OperationResult.Notfound();
			}
			ExsistedUser.LockoutEnabled = true;
			context.Update(ExsistedUser);
			await context.SaveChangesAsync();
			return OperationResult.Notfound();
		}

		public IQueryable<AdminUserViewModel> GetAll()
		{
			throw new System.NotImplementedException();
		}

		public IQueryable<AdminUserViewModel> GetBlockedUser()
		{
			throw new System.NotImplementedException();
		}

		public IQueryable<AdminUserViewModel> Search(string term)
		{
			throw new System.NotImplementedException();
		}

		public Task<int> UserRegistrationCount(int month)
		{
			throw new System.NotImplementedException();
		}
	}
}

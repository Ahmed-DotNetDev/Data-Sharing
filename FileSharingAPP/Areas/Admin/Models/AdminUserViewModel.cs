using System;

namespace FileSharingAPP.Areas.Admin.Models
{
	public class AdminUserViewModel
	{
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string Email { get; set; }
		public bool IsBlocked { get; set; }
		public DateTime CreatedDate { get; set; }

	}
}

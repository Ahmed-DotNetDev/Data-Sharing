using FileSharingAPP.Resources;
using System.ComponentModel.DataAnnotations;

namespace FileSharingAPP.Models
{

	public class LoginViewModel
	{
		[EmailAddress(ErrorMessageResourceName = "Email", ErrorMessageResourceType = typeof(SharedResource))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		public string Email { get; set; }
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "pass", ResourceType = typeof(SharedResource))]
		public string Password { get; set; }
	}

	public class RegisterViewModel
	{
		[EmailAddress(ErrorMessageResourceName = "Email", ErrorMessageResourceType = typeof(SharedResource))]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "EmailLabel", ResourceType = typeof(SharedResource))]
		public string Email { get; set; }
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "pass", ResourceType = typeof(SharedResource))]
		public string Password { get; set; }
		[Compare("Password")]
		[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(SharedResource))]
		[Display(Name = "confirmpass", ResourceType = typeof(SharedResource))]
		public string ConfirmPassword { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
	public class ChangePasswordViewModel
	{
		[Required]
		public string CurrectPassword { get; set; }
		[Required]
		public string NewPassword { get; set; }
		[Compare("NewPassword")]
		public string ConfirmNewPassword { get; set; }
	}
	public class ForgotPasswordViewMOdel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; }
	}
}

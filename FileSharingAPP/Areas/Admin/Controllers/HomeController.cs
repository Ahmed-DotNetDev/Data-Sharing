using Microsoft.AspNetCore.Mvc;

namespace FileSharingAPP.Areas.Admin.Controllers
{

	public class HomeController : AdminBaseController
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}

namespace FileSharingAPP.Areas.Admin.Models
{
	public class OperationResult
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public static OperationResult Notfound(bool suc = false, string Mes = "Not Found")
		{
			return new OperationResult { Success = suc, Message = Mes };
		}
	}
}

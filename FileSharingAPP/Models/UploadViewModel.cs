using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace FileSharingAPP.Models
{
	public class InputFile
	{
		[Required]
		public IFormFile File { get; set; }
	}
	public class InputUpload
	{
		public string OriginalName { set; get; }
		public string FileName { set; get; }
		public string ContentType { set; get; }
		public long Size { set; get; }
		public string UserId { set; get; }
	}

	public class UploadViewModel
	{
		[Display(Name = "Original Name")]

		public string OriginalName { get; set; }
		[Display(Name = "File Name")]

		public string FileName { get; set; }
		[Display(Name = "Size")]

		public decimal Size { get; set; }
		[Display(Name = "Content Type")]

		public string ContentType { get; set; }
		[Display(Name = "Upload Date")]

		public DateTime UploadDate { get; set; }
		public string Id { get; set; }
		[Display(Name = "Download Count")]
		public long DownloadCount { get; set; }
	}
}

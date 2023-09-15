using Microsoft.AspNetCore.Identity;
using System;

namespace FileSharingAPP.Data
{
    public class Uploads
    {
        public Uploads()
        {
            Id = Guid.NewGuid().ToString();
            UploadDate = DateTime.Now;
        }

        public string Id { get; set; }
        public string OriginalName { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public decimal Size { get; set; }
        public string UserId { get; set; }
        public DateTime UploadDate { get; set; }
        public IdentityUser User { get; set; }
        public long DownloadCount { get; set; }
    }
}

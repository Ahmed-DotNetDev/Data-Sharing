using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FileSharingAPP.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions options) : base(options)
		{

		}

		public DbSet<Uploads> uploads { get; set; }
		public DbSet<Contact> Contacts { get; set; }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.Entity<Uploads>().Property(u => u.Size)
				.HasColumnType("decimal(18,4)");
			base.OnModelCreating(builder);
		}

		public DbSet<FileSharingAPP.Models.UploadViewModel> UploadViewModel { get; set; }
	}
}

using FileSharingAPP.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileSharingAPP
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();
			//migration at runtime env
			using (var scope = host.Services.CreateScope())
			{
				var provider = scope.ServiceProvider;
				var DbContext = provider.GetRequiredService<ApplicationDbContext>();
				DbContext.Database.Migrate();
				//seed
			}
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}

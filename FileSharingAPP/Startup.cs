using FileSharingAPP.Data;
using FileSharingAPP.Helpers.Mail;
using FileSharingAPP.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FileSharingAPP
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews()
				.AddViewLocalization(op =>
				{
					op.ResourcesPath = "Resources";
				});
			services.AddDbContext<ApplicationDbContext>(options =>
			{
				options.UseSqlServer(Configuration.GetConnectionString("DefautConnection"));
			});
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			services.AddTransient<ImailHelper, MailHelper>();
			services.AddTransient<IUploadService, UploadService>();
			services.AddLocalization();
			services.AddAutoMapper(typeof(Startup));
			services.AddAuthentication()
				.AddFacebook(options =>
				{
					options.AppId = Configuration["Authentication:Facebook:AppId"];
					options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
				})
				.AddGoogle(options =>
				{
					options.ClientId = Configuration["Authentication:Google:ClientId"];
					options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			var SuportedCulture = new[] { "ar-SA", "en-US" };
			app.UseRequestLocalization(r =>
			{
				r.AddSupportedUICultures(SuportedCulture);
				r.AddSupportedCultures(SuportedCulture);
				r.SetDefaultCulture("en-US");
			});

			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
				name: "areas",
				pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
			  );

				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}

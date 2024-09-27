using Microsoft.AspNetCore.Authentication.Cookies;

namespace Lab1
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});

			builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/Account/Login";
				});
			builder.Services.AddAuthorization();	


			// Add services to the container.
			builder.Services.AddControllersWithViews();
            builder.Services.AddHttpClient();

            var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}

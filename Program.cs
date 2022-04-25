
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var configurationBuilder = builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("Properties/appsettings.json")
    .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json")
    .AddEnvironmentVariables()
    .Build();

var services = builder.Services;

var settingsSection = configurationBuilder.GetSection("AuthSettings");
services.Configure<AuthSettings>(settingsSection);

var settings = settingsSection.Get<AuthSettings>();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Forbidden/";
    });

services.AddAuthentication();
services.AddRazorPages();
services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

app.MapRazorPages();
app.MapDefaultControllerRoute();

// app.UseEndpoints(endpoints =>
// {
//     endpoints.MapDefaultControllerRoute();
// });


app.Run();

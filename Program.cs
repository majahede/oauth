
var builder = WebApplication.CreateBuilder(args);

var configurationBuilder = builder.Configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("Properties/appsettings.json")
    .AddJsonFile($"Properties/appsettings.{builder.Environment.EnvironmentName}.json")
    .AddEnvironmentVariables()
    .Build();

var services = builder.Services;


/*services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", config =>
    {
        config.Cookie.Name = "User.cookie";
        config.LoginPath = "/Home/Authenticate";
    });*/

 services.AddAuthentication(config =>
 {
     config.DefaultAuthenticateScheme = "ClientCookie";
     config.DefaultSignInScheme = "ClientCookie";
     config.DefaultChallengeScheme = "OurServer";
 })
     .AddCookie("ClientCookie")
     .AddOAuth("OurServer", config =>
     {
         config.CallbackPath = "/oath/callback";
         config.ClientId = "202ec997c4f67a7567a84d4f163b29d1c76e5083b92cbb127808da4d3678c464";
         config.ClientSecret = "ecb3145e58a6158796894284b83b0682404a777373a061d7165cab5898b7dbec";

     });

services.AddAuthentication();

services.AddControllersWithViews();

var app = builder.Build();


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

/*app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");*/

//app.MapRazorPages();

app.UseEndpoints(endpoints =>
{
    endpoints.MapDefaultControllerRoute();
});


app.Run();

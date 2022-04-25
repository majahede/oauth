using System.Security.Claims;
using System.Text.Json;
using assignment_wt1_oauth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace assignment_wt1_oauth.Controllers;

public class AuthController : Controller
{
    private readonly string _callbackPath;
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;

    public AuthController(IOptions<AuthSettings> settings)
    {
        _tokenEndpoint = settings.Value.TokenEndpoint;
        _callbackPath = settings.Value.CallbackPath;
        _clientId = settings.Value.ClientId;
        _clientSecret = settings.Value.ClientSecret;
    }

    public async Task<RedirectToActionResult> Callback()
    {
        var code = Request.Query["code"];

        var url =
            $"{_tokenEndpoint}?client_id={_clientId}&client_secret={_clientSecret}&code={code}&grant_type=authorization_code&redirect_uri={_callbackPath}";

        var client = new HttpClient();
        var response = await client.PostAsync(url, null);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<AuthResponse>(responseBody);

        var token = result?.access_token;

        if (token != null)
        { var identity = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Authentication, token)
            }, CookieAuthenticationDefaults.AuthenticationScheme);
            
            var principal = new ClaimsPrincipal(identity);
            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Activities", "Home");
        }

        return RedirectToAction("Index", "Home");
        // return View("~/Views/Home/Activities.cshtml");
        //Response.Redirect("https://localhost:7100/Activities");
        //   Response.Redirect($"https://localhost:7100/Activities/?token={token}");
    }
}
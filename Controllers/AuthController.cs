using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using assignment_wt1_oauth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace assignment_wt1_oauth.Controllers;
public class AuthController : Controller
{
    private readonly string _callbackPath;
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _authorizationEndpoint;
    private readonly string _scope;

    public AuthController(IOptions<AuthSettings> settings)
    {
        _authorizationEndpoint = settings.Value.AuthorizationEndpoint;
        _tokenEndpoint = settings.Value.TokenEndpoint;
        _callbackPath = settings.Value.CallbackPath;
        _clientId = settings.Value.ClientId;
        _clientSecret = settings.Value.ClientSecret;
        _scope = settings.Value.Scope;
    }

    public async Task<RedirectToActionResult> Callback()
    {
        if (Request.Query["error"] == "access_denied") return RedirectToAction("Index", "Home");
        if (Request.Query["state"] != HttpContext.Session.GetString("State")) return RedirectToAction("Error", "Home");
        
        var code = Request.Query["code"];
        var url = $"{_tokenEndpoint}?client_id={_clientId}&client_secret={_clientSecret}&code={code}&grant_type=authorization_code&redirect_uri={_callbackPath}";

        var client = new HttpClient();
        var response = await client.PostAsync(url, null);
        
        if(response.StatusCode != HttpStatusCode.OK) return RedirectToAction("Error", "Home");
        
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(responseBody); 

        var tokens = new TokenResponse()
        {
            AccessToken = result.access_token,
            IdToken = result.id_token
        };

        var handler = new JwtSecurityTokenHandler();
        var idToken = handler.ReadJwtToken(tokens.IdToken);
        
        var claims = new List<Claim>
        {
            new ("Id", idToken.Subject ), 
            new (ClaimTypes.Email, idToken.Claims.First(claim => claim.Type == "email").Value),
            new ( "AccessToken", tokens.AccessToken),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(claimsIdentity);
        
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        
        return RedirectToAction("Index", "Home");
    }
    
    public void Login()
    {
        Random r = new Random();
        String str = "abcdefghijklmnopqrstuvwxyz0123456789";
        int size = 10;
        String state = "";
  
        for (int i = 0; i < size; i++)
        {
            int x = r.Next(str.Length);
            state += str[x];
        }
        
        HttpContext.Session.SetString("State", state);
        
        Response.Redirect(
            $"{_authorizationEndpoint}?client_id={_clientId}&redirect_uri={_callbackPath}&response_type=code&state={state}&scope={_scope}");
    }
    
    public async Task<RedirectToActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
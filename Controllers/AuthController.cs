using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
    private readonly string _authorizationEndpoint;

    public AuthController(IOptions<AuthSettings> settings)
    {
        _authorizationEndpoint = settings.Value.AuthorizationEndpoint;
        _tokenEndpoint = settings.Value.TokenEndpoint;
        _callbackPath = settings.Value.CallbackPath;
        _clientId = settings.Value.ClientId;
        _clientSecret = settings.Value.ClientSecret;
    }

    public async Task<RedirectToActionResult> Callback()
    {
        var code = Request.Query["code"];

        var url = $"{_tokenEndpoint}?client_id={_clientId}&client_secret={_clientSecret}&code={code}&grant_type=authorization_code&redirect_uri={_callbackPath}";

        var client = new HttpClient();
        var response = await client.PostAsync(url, null);

        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseBody);

        var token = result?.access_token;
        var idToken = result?.id_token;

        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(idToken.ToString());
        Console.WriteLine(jwtSecurityToken);
        
        if (token == null) return RedirectToAction("Index", "Home");
      
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, jwtSecurityToken.email),
            new Claim("Id", jwtSecurityToken.sub ),
            new Claim(ClaimTypes.Expiration, jwtSecurityToken.exp),
            new Claim("AccessToken", token),
        };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);
        
        var principal = new ClaimsPrincipal(claimsIdentity);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        
        return RedirectToAction("Index", "Home");
    }
    
    public void Login()
    {
        Response.Redirect(
            $"{_authorizationEndpoint}?client_id={_clientId}&redirect_uri={_callbackPath}&response_type=code&state=12345&scope=read_api+read_user+openid+profile+email");
    }
    
    public async Task<RedirectToActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        
        return RedirectToAction("Index", "Home");
        
    }
}
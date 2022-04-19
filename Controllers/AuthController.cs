using System.Text.Json;
using assignment_wt1_oauth.Models;
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

    public async Task<ViewResult> Callback()
    {
        var code = Request.Query["code"]; 
        var url =
            $"{_tokenEndpoint}?client_id={_clientId}&client_secret={_clientSecret}&code={code}&grant_type=authorization_code&redirect_uri={_callbackPath}";
        
        var client = new HttpClient();
        var response = await client.PostAsync(url, null);
        
        response.EnsureSuccessStatusCode();
        
        var responseBody = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<AuthResponse>(responseBody);
        
        return View("~/Views/Home/Index.cshtml");
    }
}
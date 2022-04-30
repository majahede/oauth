using System.Net.Http.Headers;
using System.Security.Claims;
using assignment_wt1_oauth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace assignment_wt1_oauth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly string _callbackPath;
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _authorizationEndpoint;
    
    public HomeController(ILogger<HomeController> logger, IOptions<AuthSettings> settings)
    {
        _logger = logger;
        _tokenEndpoint = settings.Value.TokenEndpoint;
        _authorizationEndpoint = settings.Value.AuthorizationEndpoint;
        _callbackPath = settings.Value.CallbackPath;
        _clientId = settings.Value.ClientId;
        _clientSecret = settings.Value.ClientSecret;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize]
    public async Task<IActionResult> Activities()
    {
        var accessToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication)?.Value;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.GetAsync("https://gitlab.lnu.se/api/v4/events?per_page=101");
        var responseBody = await response.Content.ReadAsStringAsync();
        
       var user = await client.GetAsync($"https://gitlab.lnu.se/api/v4/user"); 
        var userresponseBody = await user.Content.ReadAsStringAsync();
       // Console.WriteLine(responseBody);
      //  Console.WriteLine(userresponseBody);
       //Event events = JsonConvert.DeserializeObject<Event>(responseBody);
     //  var events = JsonConvert.DeserializeObject<g>(responseBody);
        
       var events = JsonConvert.DeserializeObject<dynamic>(responseBody);
       var u = JsonConvert.DeserializeObject<dynamic>(userresponseBody);
       Console.WriteLine(u.email);
      // Console.WriteLine(events);
        return View();
    }

    public void Login()
    {
        Response.Redirect(
            $"{_authorizationEndpoint}?client_id={_clientId}&redirect_uri={_callbackPath}&response_type=code&state=12345&scope=read_api+read_user+openid+profile+email");
    }
}
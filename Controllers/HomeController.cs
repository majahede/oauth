using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
    
    public IActionResult Activities()
    {
        return View();
    }
    
    public void Login()
    {
        Response.Redirect(
            $"{_authorizationEndpoint}?client_id={_clientId}&redirect_uri={_callbackPath}&response_type=code&state=12345&scope=read_api");
    }
}
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace assignment_wt1_oauth.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize]
    public IActionResult Secret()
    {
        return View();
    }
    
    
    [Authorize]
    public IActionResult Code()
    {
        Console.Write("Code");
       return View();
    }
    
    public IActionResult Authenticate()
    {
        var userClaims = new List<Claim>()
        {   
            new Claim(ClaimTypes.Name, "Maja"),
            new Claim(ClaimTypes.Email, "maja@mail.com")
        };

        var licenceClaims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, "Maja"),
            new Claim(ClaimTypes.Email, "maja@mail.com")
        };
        
        var userIdentity = new ClaimsIdentity(userClaims, "User Identity");

        var userPrincipal = new ClaimsPrincipal(new[] {userIdentity});

        HttpContext.SignInAsync(userPrincipal);
        
        return RedirectToAction("Index");
    }
}
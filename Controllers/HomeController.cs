using System.Net.Http.Headers;
using System.Security.Claims;
using assignment_wt1_oauth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace assignment_wt1_oauth.Controllers;

public class HomeController : Controller
{
    private readonly string _callbackPath;
    private readonly string _clientId;
    private readonly string _authorizationEndpoint;
    
    public HomeController( IOptions<AuthSettings> settings)
    {
        _authorizationEndpoint = settings.Value.AuthorizationEndpoint;
        _callbackPath = settings.Value.CallbackPath;
        _clientId = settings.Value.ClientId;
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
       
        var response = await client.GetAsync("https://gitlab.lnu.se/api/v4/events?per_page=51");
        var responseBody = await response.Content.ReadAsStringAsync();
        var page2 = await client.GetAsync("https://gitlab.lnu.se/api/v4/events?page=2&per_page=50&sort=desc>");
        var ev = await page2.Content.ReadAsStringAsync();
        var even = responseBody + ev;

       var activities = JsonConvert.DeserializeObject<dynamic>(responseBody);
       var ac = JsonConvert.DeserializeObject<dynamic>(ev);
       var activityList = new List<Activity>();
       var jArray = new JArray(activities, ac);
       
      // Console.WriteLine(activities[100]);
       foreach (var activity in activities)
       {
           var a = new Activity()
           {
               ActionName = activity.action_name,
               CreatedAt = activity.created_at,
               TargetTitle = activity.target_title,
               TargetType = activity.target_type
           };
           
           activityList.Add(a);
       }
       Console.WriteLine(activities[100]);
        return View(activityList);
    }
    
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var accessToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Authentication)?.Value;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync($"https://gitlab.lnu.se/api/v4/user"); 
        var responseBody = await response.Content.ReadAsStringAsync();

        var user = JsonConvert.DeserializeObject<dynamic>(responseBody);
        
        ViewBag.GitlabId = user.id;
        ViewBag.Email = user.email;
        ViewBag.Name = user.name;
        ViewBag.Username = user.username;
        ViewBag.LastActivity = user.last_activity_on;
        ViewBag.Avatar = user.avatar_url;

        return View();
    }

    public void Login()
    {
        Response.Redirect(
            $"{_authorizationEndpoint}?client_id={_clientId}&redirect_uri={_callbackPath}&response_type=code&state=12345&scope=read_api+read_user+openid+profile+email");
    }
    
    public void Logout()
    {
       
    }
}
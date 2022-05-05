using System.Net.Http.Headers;
using System.Security.Claims;
using assignment_wt1_oauth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace assignment_wt1_oauth.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    
    [Authorize]
    public async Task<IActionResult> Activities()
    {
        var accessToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "AccessToken")?.Value;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var activityList = new List<Activity>();
        
        for (int i = 0; i < 2; i++)
        {
            var response = await client.GetAsync($"https://gitlab.lnu.se/api/v4/events?page={i + 1}&per_page=5{i}");
            var responseBody = await response.Content.ReadAsStringAsync();
            var activities = JsonConvert.DeserializeObject<dynamic>(responseBody);

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
        }
        return View(activityList);
    }
    
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var accessToken = HttpContext.User.Claims.FirstOrDefault(x => x.Type ==  "AccessToken")?.Value;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await client.GetAsync("https://gitlab.lnu.se/api/v4/user"); 
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
}
namespace assignment_wt1_oauth.Models;

public class AuthResponse
{
    public string access_token { get; set; } = null!;
    public string token_type { get; set; } = null!;
    public int expires_in { get; set; } 
    public string refresh_token { get; set; } = null!;
    public string scope { get; set; } = null!;
    public int created_at { get; set; } 

}
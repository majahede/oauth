namespace assignment_wt1_oauth.Models;

public class AuthSettings
{
    public string CallbackPath { get; set; } = null!;

    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string AuthorizationEndpoint { get; set; } = null!;
    
    public string TokenEndpoint { get; set; } = null!;
    
    public string Scope { get; set; } = null!;
    
}
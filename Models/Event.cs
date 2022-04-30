namespace assignment_wt1_oauth.Models;

public class Author
{
    public string name { get; set; }
    public string username { get; set; }
    public int id { get; set; }
    public string state { get; set; }
    public string avatar_url { get; set; }
    public string web_url { get; set; }
}

public class Event
{
    public int id { get; set; }
    public object title { get; set; }
    public int project_id { get; set; }
    public string action_name { get; set; }
    public int target_id { get; set; }
    public string target_type { get; set; }
    public int author_id { get; set; }
    public string target_title { get; set; }
    public DateTime created_at { get; set; }
    public Author author { get; set; }
    public string author_username { get; set; }
}
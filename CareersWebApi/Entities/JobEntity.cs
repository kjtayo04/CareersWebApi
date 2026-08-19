namespace CareersWebApi.Entities;

public class JobEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public string AbsoluteUrl { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

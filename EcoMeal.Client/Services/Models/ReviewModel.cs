namespace EcoMeal.Site.Models;

public class ReviewModel
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
    public string UserName { get; set; } = "";
    public string PackageName { get; set; } = "";
}

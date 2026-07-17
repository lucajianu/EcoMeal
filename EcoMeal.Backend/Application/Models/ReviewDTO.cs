namespace EcoMeal.Backend.Models;
public class ReviewDTO
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
    public string UserName { get; set; } = "";
    public string PackageName { get; set; } = "";
}

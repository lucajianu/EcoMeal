namespace EcoMeal.Site.Models;

public class PackageGetModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public double Price { get; set; }
    public DateTime StartPickup { get; set; }
    public DateTime EndPickup { get; set; }
    public int PackageTypeId { get; set; }
    public string PackageTypeName { get; set; } = "";
}

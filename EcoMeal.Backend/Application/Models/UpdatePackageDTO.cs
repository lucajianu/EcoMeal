namespace EcoMeal.Backend.Models;

public class UpdatePackageDTO
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public double Price { get; set; }
    public int NoPackage { get; set; }
    public DateTime StartPickup { get; set; }
    public DateTime EndPickup { get; set; }
    public int PackageTypeId { get; set; }
}

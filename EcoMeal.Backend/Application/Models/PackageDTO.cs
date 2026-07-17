namespace EcoMeal.Backend.Models;

public class PackageDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public double Price { get; set; }
    public int NoPackage { get; set; }
    public int Available { get; set; }
    public DateTime StartPickup { get; set; }
    public DateTime EndPickup { get; set; }
    public int PackageTypeId { get; set; }
    public required string PackageTypeName { get; set; }
    public string? ImageUrl { get; set; }
}

namespace EcoMeal.Backend.Models;

public class BusinessDetailsDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? Description { get; set; }
    public required string Contact { get; set; }
    public int BusinessTypeId { get; set; }
    public required string BusinessTypeName { get; set; }
    public string? ImageUrl { get; set; }
    public double? Rating { get; set; }
    public int ReviewCount { get; set; }
    public List<PackageDTO> Packages { get; set; } = new();
    public List<ReviewDTO> Reviews { get; set; } = new();
}

using EcoMeal.Backend.Entities;

public class Package
{
    public int Id{get;set;}
    public required string Name{get;set;}
    public int NoPackage{get;set;}
    public int BusinessId{get;set;}
    public Business Business{get;set;} = null!;
    public required int PackageTypeId{get;set;}
    public PackageType PackageType{get;set;} = null!;
    public string? Description{get;set;}
    public string? ImagePath{get;set;}
    public required double Price{get;set;}
    public required DateTime StartPickup{get;set;}
    public required DateTime EndPickup{get;set;}
    public ICollection<Order> Orders{get;set;} = new List<Order>();
}

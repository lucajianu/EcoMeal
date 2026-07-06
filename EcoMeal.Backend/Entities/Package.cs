using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

public class Package
{
    public  required int  Id{get;set;}
    public required string Name{get;set;}
    public  required int NoPackage{get;set;}
    public  required int  BusinessId{get;set;}
    public required Business Business{get;set;}
    public required  int PackageTypeId{get;set;}
    public  required PackageType PackageType{get;set;}
    public string? Description{get;set;}
    public  required decimal Price{get;set;}
    public required  DateTime StartPickup{get;set;}
    public required  DateTime LastPickup{get;set;}
    public ICollection<Order> Orders=new List<Order>();
}
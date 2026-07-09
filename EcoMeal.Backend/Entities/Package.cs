using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

public class Package
{
    public   int  Id{get;set;}
    public required string Name{get;set;}
    public   int NoPackage{get;set;}
    public    int  BusinessId{get;set;}
    public   Business Business{get;set;}
    public required  int PackageTypeId{get;set;}
    public   PackageType PackageType{get;set;}
    public string? Description{get;set;}
    public  required double Price{get;set;}
    public required  DateTime StartPickup{get;set;}
    public required  DateTime EndPickup{get;set;}
    public ICollection<Order> Orders=new List<Order>();
}
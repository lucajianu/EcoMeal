using EcoMeal.Backend.Entities;

public enum State
{
    PENDING, 
    ACCEPTED,
    REJECT, 
    FINISHED, 
    CANCELLED
}

public class Order
{
    public  int Id{get;set;}
    public required int UserId{get;set;}
    public required User User{get;set;}
    public required int PackageId{get;set;}
    public required Package Package {get;set;}
    public required State Status{get;set;}
    public DateTime? PlacedAt{get;set;}
    

}
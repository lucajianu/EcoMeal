
namespace EcoMeal.Backend.Entities;
public enum State
{
    Pending,
    Accepted,
    Rejected,
    Finished,
    Cancelled
}

public class Order
{
    public int Id{get;set;}
    public int Count{get;set;}
    public int UserId{get;set;}
    public int PackageId{get;set;}
    public State Status{get;set;} = State.Pending;
    public DateTime? Date{get;set;}
    public Package? Package {get;set;}
    public User? User{get;set;}
}

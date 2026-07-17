namespace EcoMeal.Backend.Entities;

// o recenzie apartine unei singure comenzi (finalizate);
// business-ul se afla prin Order -> Package -> Business, fara FK duplicat
public class Review
{
    public int Id{get;set;}
    public int OrderId{get;set;}
    public Order? Order{get;set;}
    public int Rating{get;set;}
    public string? Comment{get;set;}
    public DateTime Date{get;set;}
}

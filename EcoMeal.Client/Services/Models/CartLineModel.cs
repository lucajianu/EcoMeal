namespace EcoMeal.Site.Models;

// o linie din cos = o viitoare comanda de sine statatoare
public class CartLineModel
{
    public int PackageId { get; set; }
    public string PackageName { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public double Price { get; set; }
    public int Available { get; set; }
    public int Count { get; set; }
    public double LineTotal => Price * Count;
}

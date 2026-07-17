namespace EcoMeal.Backend.Models;

// payload-ul notificarilor SignalR; Status e string ca sa nu depindem
// de setarile de serializare ale hub-ului pentru enum-uri
public class OrderNotificationDTO
{
    public int OrderId { get; set; }
    public string PackageName { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public string Status { get; set; } = "";
    public int Count { get; set; }
    public string UserName { get; set; } = "";
}

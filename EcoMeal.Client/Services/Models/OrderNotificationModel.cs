namespace EcoMeal.Site.Models;

// oglinda OrderNotificationDTO din backend (payload-ul evenimentelor SignalR)
public class OrderNotificationModel
{
    public int OrderId { get; set; }
    public string PackageName { get; set; } = "";
    public string BusinessName { get; set; } = "";
    public string Status { get; set; } = "";
    public int Count { get; set; }
    public string UserName { get; set; } = "";
}

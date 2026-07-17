using EcoMeal.Site.Models;

namespace EcoMeal.Site.Services;

public class NotificationItem
{
    public string Message { get; init; } = "";
    public string CssClass { get; init; } = "";
    public DateTime Date { get; init; } = DateTime.Now;
    public bool IsRead { get; set; }
}

// aduna notificarile primite prin SignalR intr-o lista pentru dropdown-ul din navbar;
// evenimentele vin deja filtrate pe roluri (grupurile hub-ului), deci lista e
// automat cea potrivita: adminul vede comenzi noi/anulari, clientul vede statusuri
public class NotificationCenter : IDisposable
{
    private const int MaxItems = 50;

    private readonly OrderNotificationService _orderNotifications;
    private readonly List<NotificationItem> _items = new();

    public IReadOnlyList<NotificationItem> Items => _items;
    public int UnreadCount => _items.Count(i => !i.IsRead);
    public event Action? OnChange;

    public NotificationCenter(OrderNotificationService orderNotifications)
    {
        _orderNotifications = orderNotifications;
        _orderNotifications.OrderPlaced += HandleOrderPlaced;
        _orderNotifications.OrderStatusChanged += HandleOrderStatusChanged;
    }

    private void HandleOrderPlaced(OrderNotificationModel n)
        => Add($"Comandă nouă: {n.PackageName} x{n.Count} la {n.BusinessName} (de la {n.UserName})", "text-primary");

    private void HandleOrderStatusChanged(OrderNotificationModel n)
    {
        var (text, css) = n.Status switch
        {
            "Accepted" => ($"Comanda pentru „{n.PackageName}” a fost acceptată", "text-success"),
            "Rejected" => ($"Comanda pentru „{n.PackageName}” a fost respinsă", "text-danger"),
            "Finished" => ($"Comanda pentru „{n.PackageName}” a fost finalizată", "text-success"),
            "Cancelled" => ($"Comanda pentru „{n.PackageName}” ({n.BusinessName}) a fost anulată de client", "text-warning"),
            _ => ($"Comanda pentru „{n.PackageName}”: {n.Status}", "text-secondary")
        };
        Add(text, css);
    }

    private void Add(string message, string cssClass)
    {
        _items.Insert(0, new NotificationItem { Message = message, CssClass = cssClass });
        if (_items.Count > MaxItems)
        {
            _items.RemoveAt(_items.Count - 1);
        }
        OnChange?.Invoke();
    }

    public void MarkAllRead()
    {
        foreach (var item in _items)
        {
            item.IsRead = true;
        }
        OnChange?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        OnChange?.Invoke();
    }

    public void Dispose()
    {
        _orderNotifications.OrderPlaced -= HandleOrderPlaced;
        _orderNotifications.OrderStatusChanged -= HandleOrderStatusChanged;
    }
}

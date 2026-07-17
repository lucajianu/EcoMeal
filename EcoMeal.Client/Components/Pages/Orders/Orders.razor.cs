using EcoMeal.Client.Models;
using EcoMeal.Site.Models;
using EcoMeal.Site.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.Client.Components.Pages.Orders;
public partial class Orders : IDisposable
{
        [Inject]
        public required OrderService OrderService{get;set;}
        [Inject]
        public required OrderNotificationService OrderNotifications{get;set;}
        private List<OrderGetModel>? MyOrders;

        protected override void OnInitialized()
        {
            // cand business-ul schimba statusul, lista se actualizeaza singura
            OrderNotifications.OrderStatusChanged += HandleStatusChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await ReloadOrdersAsync();
            StateHasChanged();
        }
    }

    private async Task ReloadOrdersAsync()
    {
        try
        {
            MyOrders = await OrderService.GetMyOrdersAsync();
        }
        catch (HttpRequestException)
        {
            MyOrders = new List<OrderGetModel>();
        }
    }

    private void HandleStatusChanged(OrderNotificationModel notification)
    {
        _ = InvokeAsync(async () =>
        {
            await ReloadOrdersAsync();
            StateHasChanged();
        });
    }

    private async Task CancelOrder(int orderId)
    {
        var success = await OrderService.UpdateStatusAsync(orderId, "Cancelled");
        if (success)
        {
            await ReloadOrdersAsync();
        }
    }

    public void Dispose()
    {
        OrderNotifications.OrderStatusChanged -= HandleStatusChanged;
    }

    private static string StatusBadgeClass(string status) => status switch
    {
        "Pending" => "bg-warning-subtle text-warning-emphasis",
        "Accepted" => "bg-info-subtle text-info-emphasis",
        "Finished" => "bg-success-subtle text-success-emphasis",
        "Rejected" => "bg-danger-subtle text-danger-emphasis",
        "Cancelled" => "bg-secondary-subtle text-secondary-emphasis",
        _ => "bg-light text-dark"
    };
}

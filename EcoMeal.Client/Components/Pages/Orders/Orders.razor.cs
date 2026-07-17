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
        private int? ReviewingOrderId;
        private int DraftRating;
        private string DraftComment = "";
        private string? ReviewError;

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

    private void StartReview(int orderId)
    {
        ReviewingOrderId = orderId;
        DraftRating = 5;
        DraftComment = "";
        ReviewError = null;
    }

    private async Task SubmitReview(int orderId)
    {
        var comment = string.IsNullOrWhiteSpace(DraftComment) ? null : DraftComment.Trim();
        ReviewError = await OrderService.SubmitReviewAsync(orderId, DraftRating, comment);
        if (ReviewError is null)
        {
            ReviewingOrderId = null;
            await ReloadOrdersAsync();
        }
    }

    private static string Stars(int rating)
        => new string('★', rating) + new string('☆', 5 - rating);

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

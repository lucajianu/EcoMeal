using System.Security.Claims;
using EcoMeal.Backend.Application.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace EcoMeal.Backend.Application.Hubs;

// hub-ul nu are metode apelabile de client: serverul doar trimite notificari
// fiecare conexiune intra in grupul propriu ("user-{id}"), adminii si in "admins"
[Authorize]
public class OrderHub : Hub
{
    public const string AdminsGroup = "admins";
    public static string UserGroup(int userId) => $"user-{userId}";

    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is not null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }
        if (Context.User?.IsInRole(UserRoles.Admin) == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, AdminsGroup);
        }
        await base.OnConnectedAsync();
    }
}

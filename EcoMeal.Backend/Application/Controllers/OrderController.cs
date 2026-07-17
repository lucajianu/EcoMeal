using System.Security.Claims;
using EcoMeal.Backend.Application.Constants;
using EcoMeal.Backend.Application.Hubs;
using EcoMeal.Backend.Entities;
using EcoMeal.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Backend.Application;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly EcoMealDbContext _context;
    private readonly IHubContext<OrderHub> _orderHub;
    public OrderController(EcoMealDbContext context, IHubContext<OrderHub> orderHub)
    {
        _context=context;
        _orderHub=orderHub;
    }
    // fiecare item din cos devine o comanda de sine statatoare;
    // totul se valideaza intai, apoi se salveaza intr-o singura tranzactie
    [HttpPost("checkout")]
    public async Task<ActionResult<IEnumerable<OrderGetDTO>>> Checkout([FromBody] CheckoutDTO request)
    {
        var userId=GetCurrentUserId();
        var packageIds=request.Items.Select(i=>i.PackageId).ToList();
        var packages=await _context.Packages
        .Include(p=>p.Business)
        .Include(p=>p.Orders)
        .Where(p=>packageIds.Contains(p.Id))
        .ToListAsync();

        var errors=new List<string>();
        // acelasi pachet pe mai multe linii conteaza cumulat la stoc
        foreach (var group in request.Items.GroupBy(i=>i.PackageId))
        {
            var package=packages.FirstOrDefault(p=>p.Id==group.Key);
            if (package is null)
            {
                errors.Add($"Un pachet din cos nu mai exista (id {group.Key})");
                continue;
            }
            var reserved=package.Orders
            .Where(o=>o.Status!=State.Cancelled && o.Status!=State.Rejected)
            .Sum(o=>o.Count);
            var available=package.NoPackage-reserved;
            var requested=group.Sum(i=>i.Count);
            if (requested>available)
            {
                errors.Add(available<=0
                    ? $"„{package.Name}” nu mai este disponibil"
                    : $"„{package.Name}”: doar {available} mai sunt disponibile");
            }
        }
        if (errors.Any())
        {
            return BadRequest(string.Join("\n", errors));
        }

        var user=await _context.Users.FindAsync(userId);
        var orders=request.Items.Select(item=>new Order
        {
            UserId=userId,
            PackageId=item.PackageId,
            Count=item.Count,
            Status=State.Pending,
            Date=DateTime.UtcNow,
        }).ToList();
        _context.Orders.AddRange(orders);
        // un singur SaveChanges = o singura tranzactie: ori toate comenzile, ori niciuna
        await _context.SaveChangesAsync();

        var result=new List<OrderGetDTO>();
        foreach (var order in orders)
        {
            var package=packages.First(p=>p.Id==order.PackageId);
            // business-ul (deocamdata adminul) afla imediat ca a venit o comanda noua
            await _orderHub.Clients.Group(OrderHub.AdminsGroup).SendAsync("OrderPlaced", new OrderNotificationDTO
            {
                OrderId = order.Id,
                PackageName = package.Name,
                BusinessName = package.Business.Name,
                Status = order.Status.ToString(),
                Count = order.Count,
                UserName = user?.Name ?? ""
            });
            result.Add(new OrderGetDTO
            {
                Id=order.Id,
                UserId=order.UserId,
                Date=order.Date,
                Status=order.Status,
                Count=order.Count,
                PackageId=package.Id,
                PackageName=package.Name,
                Price=package.Price,
                BusinessId=package.BusinessId,
                BusinessName=package.Business.Name,
                UserName=user?.Name ?? "",
                UserContact=user?.Contact ?? ""
            });
        }
        return Ok(result);
    }

        private int GetCurrentUserId()
        {
            var userIdValue=User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdValue!);
        }
        [HttpGet("my")]
        public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetMyOrders()
    {
        var userId=GetCurrentUserId();
        var orders=await _context.Orders
        .Where(o=>o.UserId==userId)
        .OrderByDescending(o=>o.Date)
        .Select(o=>new OrderGetDTO{
           Id=o.Id,
           UserId=o.UserId,
           Date=o.Date,
            Status=o.Status,
            Count=o.Count,
            Price=o.Package!.Price,
            PackageId=o.PackageId,
            BusinessId=o.Package.BusinessId,
            BusinessName=o.Package.Business.Name,
            PackageName=o.Package.Name
        }).ToListAsync() ;

        return Ok(orders);

    }

    // comenzile primite de business-uri; adminul le gestioneaza (accepta/respinge/finalizeaza)
    [HttpGet("business")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<ActionResult<IEnumerable<OrderGetDTO>>> GetBusinessOrders([FromQuery] int? businessId)
    {
        var query = _context.Orders.AsQueryable();
        if (businessId is not null)
        {
            query = query.Where(o => o.Package!.BusinessId == businessId);
        }
        var orders = await query
            .OrderByDescending(o => o.Date)
            .Select(o => new OrderGetDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                Date = o.Date,
                Status = o.Status,
                Count = o.Count,
                Price = o.Package!.Price,
                PackageId = o.PackageId,
                BusinessId = o.Package.BusinessId,
                BusinessName = o.Package.Business.Name,
                PackageName = o.Package.Name,
                UserName = o.User!.Name ?? "",
                UserContact = o.User.Contact ?? ""
            }).ToListAsync();

        return Ok(orders);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDTO request)
    {
        var order = await _context.Orders
            .Include(o => o.Package!)
            .ThenInclude(p => p.Business)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == id);
        if (order is null)
        {
            return NotFound();
        }

        if (request.Status == State.Cancelled)
        {
            // doar clientul isi poate anula comanda, si doar cat timp e in asteptare
            if (order.UserId != GetCurrentUserId())
            {
                return Forbid();
            }
            if (order.Status != State.Pending)
            {
                return BadRequest("Comanda poate fi anulata doar cat timp este in asteptare");
            }
        }
        else
        {
            if (!User.IsInRole(UserRoles.Admin))
            {
                return Forbid();
            }
            var allowed = order.Status switch
            {
                State.Pending => request.Status is State.Accepted or State.Rejected,
                State.Accepted => request.Status == State.Finished,
                _ => false
            };
            if (!allowed)
            {
                return BadRequest($"Tranzitie invalida: {order.Status} -> {request.Status}");
            }
        }

        order.Status = request.Status;
        await _context.SaveChangesAsync();

        var notification = new OrderNotificationDTO
        {
            OrderId = order.Id,
            PackageName = order.Package!.Name,
            BusinessName = order.Package.Business.Name,
            Status = order.Status.ToString(),
            Count = order.Count,
            UserName = order.User?.Name ?? ""
        };
        if (request.Status == State.Cancelled)
        {
            // clientul si-a anulat comanda: business-ul trebuie sa afle
            await _orderHub.Clients.Group(OrderHub.AdminsGroup).SendAsync("OrderStatusChanged", notification);
        }
        else
        {
            // business-ul a schimbat statusul: clientul trebuie sa afle
            await _orderHub.Clients.Group(OrderHub.UserGroup(order.UserId)).SendAsync("OrderStatusChanged", notification);
        }
        return NoContent();
    }
}

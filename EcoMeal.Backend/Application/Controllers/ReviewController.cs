using System.Security.Claims;
using EcoMeal.Backend.Entities;
using EcoMeal.Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoMeal.Backend.Application;
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewController : ControllerBase
{
    private readonly EcoMealDbContext _context;
    public ReviewController(EcoMealDbContext context)
    {
        _context=context;
    }

    // regulile sunt verificate pe server, nu doar ascunse in UI:
    // doar comanda ta, doar finalizata, o singura recenzie
    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] ReviewCreateDTO request)
    {
        var userId=int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var order=await _context.Orders
        .Include(o=>o.Review)
        .FirstOrDefaultAsync(o=>o.Id==request.OrderId);

        if (order is null)
        {
            return NotFound("Comanda nu a fost gasita");
        }
        if (order.UserId!=userId)
        {
            return Forbid();
        }
        if (order.Status!=State.Finished)
        {
            return BadRequest("Poti lasa o recenzie doar dupa ce comanda a fost finalizata");
        }
        if (order.Review is not null)
        {
            return BadRequest("Ai lasat deja o recenzie pentru aceasta comanda");
        }

        _context.Reviews.Add(new Review
        {
            OrderId=order.Id,
            Rating=request.Rating,
            Comment=request.Comment,
            Date=DateTime.UtcNow
        });
        await _context.SaveChangesAsync();
        return Created();
    }
}

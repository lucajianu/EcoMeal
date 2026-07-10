using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EcoMeal.Backend.Application;
using EcoMeal.Backend.Models;

[ApiController]
[Route("api/[controller]")]
public class PackageTypesController : ControllerBase
{
    private readonly EcoMealDbContext _context;
    public PackageTypesController(EcoMealDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PackageTypeDTO>>> GetAll()
    {
        var types = await _context.PackageTypes.Select(t => new PackageTypeDTO
        {
            Id = t.Id,
            Name = t.Name
        }).ToListAsync();
        return Ok(types);
    }
}

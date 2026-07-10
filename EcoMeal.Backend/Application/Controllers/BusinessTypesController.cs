using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EcoMeal.Backend.Application;
using EcoMeal.Backend.Models;

[ApiController]
[Route("api/[controller]")]
public class BusinessTypesController : ControllerBase
{
    private readonly EcoMealDbContext _context;
    public BusinessTypesController(EcoMealDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusinessTypeDTO>>> GetAll()
    {
        var types = await _context.BusinessTypes.Select(t => new BusinessTypeDTO
        {
            Id = t.Id,
            Name = t.Name
        }).ToListAsync();
        return Ok(types);
    }
}

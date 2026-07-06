using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.ComponentModel;
namespace EcoMeal.Backend.Application;
[ApiController]
[Route("api/[controller]")]
public class BusinessController: ControllerBase
{
    private readonly EcoMealDbContext _context;//daca e privata _, daca nu fara
    public BusinessController(EcoMealDbContext context)
    {
        _context=context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusinessDTO>>> GetAll()
    {
        var businessesDTOs= await _context.Businesses.Include(b=>b.BusinessType).Select(b=> new BusinessDTO
        {
            Id= b.Id,
            Name=b.Name,
            Address=b.Address,
            Description=b.Description,
            Contact=b.Contact,
            BusinessTypeName=b.BusinessType.Name
        } ).ToListAsync();
        return Ok(businessesDTOs);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var business=await _context.Businesses.FindAsync(id);
        if(business is null)
        {
            return NotFound();//404, 500=erori ale serverului, 200=OK
        }
        _context.Businesses.Remove(business);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    
}
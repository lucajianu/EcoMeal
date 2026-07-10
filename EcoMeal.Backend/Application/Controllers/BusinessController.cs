using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.ComponentModel;
namespace EcoMeal.Backend.Application;
using  EcoMeal.Backend.Models;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.Extensions.DependencyModel.Resolution;

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
    
    [HttpGet("{id}")]
    public async Task<ActionResult<BusinessDetailsDTO>> GetOneById(int id)
    {
        var business = await _context.Businesses
            .Select(b => new BusinessDetailsDTO
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Description = b.Description,
                Contact = b.Contact,
                BusinessTypeId = b.BusinessTypeId,
                BusinessTypeName = b.BusinessType.Name,
                Packages = b.Packages.Select(p => new PackageDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StartPickup = p.StartPickup,
                    EndPickup = p.EndPickup,
                    PackageTypeId = p.PackageTypeId,
                    PackageTypeName = p.PackageType.Name
                }).ToList()
            })
            .FirstOrDefaultAsync(b => b.Id == id);
        if (business is null)
        {
            return NotFound();
        }

        return Ok(business);
    }
    [HttpPost]
    public async Task<IActionResult> AddBusiness([FromBody] BusinessAddDTO business)
    {
        var typeExists = await _context.BusinessTypes.AnyAsync(t => t.Id == business.BusinessTypeId);
        if (!typeExists)
        {
            return BadRequest("Unknown BusinessTypeId");
        }
        var bus = new Business
        {
            Name = business.Name,
            Address = business.Address,
            Description = business.Description,
            Contact = business.Contact,
            BusinessTypeId = business.BusinessTypeId
        };
        _context.Businesses.Add(bus);
        await _context.SaveChangesAsync();
        return Created();  
         }
    [HttpPut("{id}")]
    public async Task<IActionResult> EditBusiness(int id, [FromBody] UpdateBusinessDTO business)
    {
        var updatedBusiness = await _context.Businesses.FindAsync(id);
        if (updatedBusiness is null)
        {
            return NotFound();
        }
        var typeExists = await _context.BusinessTypes.AnyAsync(t => t.Id == business.BusinessTypeId);
        if (!typeExists)
        {
            return BadRequest("Unknown BusinessTypeId");
        }
        updatedBusiness.Name = business.Name;
        updatedBusiness.Address = business.Address;
        updatedBusiness.Description = business.Description;
        updatedBusiness.Contact = business.Contact;
        updatedBusiness.BusinessTypeId = business.BusinessTypeId;
        await _context.SaveChangesAsync();
        return NoContent();
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



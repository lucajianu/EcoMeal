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
    [HttpGet("{id}")]
    public async Task<ActionResult<BusinessDetailsDTO>> GetOneById(int id)
    {
        var business = await _context.Businesses
            .Include(b => b.Packages)
            .ThenInclude(p => p.PackageType)
            .Select(b => new BusinessDetailsDTO
            {
                Id = b.Id,
                Name = b.Name,
                Address = b.Address,
                Description = b.Description,
                Contact = b.Contact,
                BusinessTypeName = b.BusinessType.Name,
            })
            .FirstOrDefaultAsync(b => b.Id == id);
        if (business is null)
        {
            return NotFound();
        }

        return Ok(business);
    }
    [HttpPost]
    [Route("{id}/addPackage")]
    public async  Task<IActionResult> AddPackageToBusiness(int id, [FromBody] PackageAddDTO package )
    {
        _context.Packages.Add(new Package
        {
            Name=package.Name,
            Description=package.Description,
            Price=package.Price,
            StartPickup=package.StartPickup,
            EndPickup=package.EndPickup,
            PackageTypeId=package.PackageTypeId,
            BusinessId=id
        });
        await _context.SaveChangesAsync();
        return Created();
    }
    [HttpPut]
    [Route("{id}/addPackage")]

    public  async Task<IActionResult> EditPackage(int id,[FromBody]PackageAddDTO package)
    {
        var pack= await _context.Packages.FindAsync(id);
        if(pack==null)
            return NotFound();
        else
        {
           pack.Name=
        }
        
        return NoContent();
    }




}



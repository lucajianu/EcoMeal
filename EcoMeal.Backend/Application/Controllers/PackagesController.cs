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
public class PackagesController : ControllerBase
{
    private readonly EcoMealDbContext _context;
    public PackagesController(EcoMealDbContext context)
    {
        _context=context;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PackageDTO>>> GetAll()
    {
      var packagesDTOs= await _context.Packages.Include(p=>p.PackageType).Select(p=> new PackageDTO
        {
            Id=p.Id,
            Name=p.Name,
            Description=p.Description,
            Price=p.Price,
            StartPickup=p.StartPickup,
            EndPickup=p.EndPickup,
            PackageTypeId=p.PackageTypeId,
            PackageTypeName=p.PackageType.Name
        } ).ToListAsync();
        return Ok(packagesDTOs);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<PackageDTO>> GetPackageById(int id)
    {
        var package=await _context.Packages.Include(p=>p.PackageType).Select(p=> new PackageDTO
        {
            Id=p.Id,
            Name=p.Name,
            Description=p.Description,
            Price=p.Price,
            StartPickup=p.StartPickup,
            EndPickup=p.EndPickup,
            PackageTypeId=p.PackageTypeId,
            PackageTypeName=p.PackageType.Name
        } ).FirstOrDefaultAsync(p=>p.Id==id);
        if(package is null)
            return NotFound();
        return Ok(package);
    }
    [HttpPost]
    public async Task<IActionResult> AddPackage([FromBody] PackageAddDTO package)
    {

         _context.Packages.Add(new Package
        {
            Name=package.Name,
            Description=package.Description,
            Price=package.Price,
            StartPickup=package.StartPickup,
            EndPickup=package.EndPickup,
            PackageTypeId=package.PackageTypeId,
            BusinessId=package.BusinessId
        });
        await _context.SaveChangesAsync();
        return Created();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePackageAtId(int id,[FromBody] UpdatePackageDTO package)
    {
        var updatedpackage= await _context.Packages.FindAsync(id);
        if(updatedpackage is null)
            return NotFound();
        
            updatedpackage.Name=package.Name;
            updatedpackage.Description=package.Description;
            updatedpackage.Price=package.Price;
            updatedpackage.StartPickup=package.StartPickup;
            updatedpackage.EndPickup=package.EndPickup;
            updatedpackage.PackageTypeId=package.PackageTypeId;
             await  _context.SaveChangesAsync();
            return NoContent();
           
       
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        var package=await _context.Packages.FindAsync(id);
        if(package is null)
        {
            return NotFound();//404, 500=erori ale serverului, 200=OK
        }
        _context.Packages.Remove(package);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
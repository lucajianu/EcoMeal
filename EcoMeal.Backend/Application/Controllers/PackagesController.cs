using EcoMeal.Backend.Entities;
using EcoMeal.Backend.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace EcoMeal.Backend.Application;
using  EcoMeal.Backend.Models;
using EcoMeal.Backend.Application.Constants;
[ApiController]
[Route("api/[controller]")]
public class PackagesController : ControllerBase
{
    private readonly EcoMealDbContext _context;
    private readonly ImageStorage _imageStorage;
    public PackagesController(EcoMealDbContext context, ImageStorage imageStorage)
    {
        _context=context;
        _imageStorage=imageStorage;
    }
    // imaginile sunt servite de backend, deci clientul are nevoie de URL-ul absolut
    private string? ToImageUrl(string? imagePath)
        => imagePath is null ? null : $"{Request.Scheme}://{Request.Host}{imagePath}";
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PackageDTO>>> GetAll()
    {
      var packagesDTOs= await _context.Packages.Include(p=>p.PackageType).Select(p=> new PackageDTO
        {
            Id=p.Id,
            Name=p.Name,
            Description=p.Description,
            Price=p.Price,
            NoPackage=p.NoPackage,
            Available=p.NoPackage-p.Orders
                .Where(o=>o.Status!=State.Cancelled && o.Status!=State.Rejected)
                .Sum(o=>o.Count),
            StartPickup=p.StartPickup,
            EndPickup=p.EndPickup,
            PackageTypeId=p.PackageTypeId,
            PackageTypeName=p.PackageType.Name,
            ImageUrl=p.ImagePath
        } ).ToListAsync();
        foreach (var dto in packagesDTOs)
        {
            dto.ImageUrl=ToImageUrl(dto.ImageUrl);
        }
        return Ok(packagesDTOs);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<PackageDTO>> GetPackageById(int id)
    {
        var package=await _context.Packages
        .Include(p=>p.PackageType)
        .Select(p=> new PackageDTO
        {
            Id=p.Id,
            Name=p.Name,
            Description=p.Description,
            Price=p.Price,
            NoPackage=p.NoPackage,
            Available=p.NoPackage-p.Orders
                .Where(o=>o.Status!=State.Cancelled && o.Status!=State.Rejected)
                .Sum(o=>o.Count),
            StartPickup=p.StartPickup,
            EndPickup=p.EndPickup,
            PackageTypeId=p.PackageTypeId,
            PackageTypeName=p.PackageType.Name,
            ImageUrl=p.ImagePath
        } ).FirstOrDefaultAsync(p=>p.Id==id);
        if(package is null)
            return NotFound();
        package.ImageUrl=ToImageUrl(package.ImageUrl);
        return Ok(package);
    }
    [HttpPost]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> AddPackage([FromBody] PackageAddDTO package)
    {

        var newPackage = new Package
        {
            Name=package.Name,
            Description=package.Description,
            Price=package.Price,
            NoPackage=package.NoPackage,
            StartPickup=package.StartPickup,
            EndPickup=package.EndPickup,
            PackageTypeId=package.PackageTypeId,
            BusinessId=package.BusinessId
        };
        _context.Packages.Add(newPackage);
        await _context.SaveChangesAsync();
        // clientul are nevoie de Id ca sa poata urca imaginea imediat dupa creare
        return CreatedAtAction(nameof(GetPackageById), new { id = newPackage.Id }, new { newPackage.Id });
    }

    [HttpPost("{id}/image")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var package = await _context.Packages.FindAsync(id);
        if (package is null)
        {
            return NotFound();
        }
        string newPath;
        try
        {
            newPath = await _imageStorage.SaveAsync(file, "packages");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        _imageStorage.Delete(package.ImagePath);
        package.ImagePath = newPath;
        await _context.SaveChangesAsync();
        return Ok(new { ImageUrl = ToImageUrl(newPath) });
    }
    [HttpPut("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UpdatePackageAtId(int id,[FromBody] UpdatePackageDTO package)
    {
        var updatedpackage= await _context.Packages.FindAsync(id);
        if(updatedpackage is null)
            return NotFound();
        
            updatedpackage.Name=package.Name;
            updatedpackage.Description=package.Description;
            updatedpackage.Price=package.Price;
            updatedpackage.NoPackage=package.NoPackage;
            updatedpackage.StartPickup=package.StartPickup;
            updatedpackage.EndPickup=package.EndPickup;
            updatedpackage.PackageTypeId=package.PackageTypeId;
             await  _context.SaveChangesAsync();
            return NoContent();
           
    }//poate sa fie si PATCH
    
    [HttpDelete("{id}")]
    [Authorize(Roles = UserRoles.Admin)]
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EcoMeal.Backend.Application.Constants;
using EcoMeal.Backend.Entities;
using EcoMeal.Backend.Infrastructure;
using  EcoMeal.Backend.Models;
using Microsoft.AspNetCore.Authorization;
namespace EcoMeal.Backend.Application;
[ApiController]
[Route("api/[controller]")]
public class BusinessController: ControllerBase
{
    private readonly EcoMealDbContext _context;//daca e privata _, daca nu fara
    private readonly ImageStorage _imageStorage;
    public BusinessController(EcoMealDbContext context, ImageStorage imageStorage)
    {
        _context=context;
        _imageStorage=imageStorage;
    }
    // imaginile sunt servite de backend, deci clientul are nevoie de URL-ul absolut
    private string? ToImageUrl(string? imagePath)
        => imagePath is null ? null : $"{Request.Scheme}://{Request.Host}{imagePath}";

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
            ImageUrl=b.ImagePath,
            BusinessTypeName=b.BusinessType.Name
        } ).ToListAsync();
        foreach (var dto in businessesDTOs)
        {
            dto.ImageUrl=ToImageUrl(dto.ImageUrl);
        }
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
                ImageUrl = b.ImagePath,
                Packages = b.Packages.Select(p => new PackageDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImagePath,
                    NoPackage = p.NoPackage,
                    Available = p.NoPackage - p.Orders
                        .Where(o => o.Status != State.Cancelled
                                 && o.Status != State.Rejected)
                        .Sum(o => o.Count),
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
        business.ImageUrl=ToImageUrl(business.ImageUrl);
        foreach (var package in business.Packages)
        {
            package.ImageUrl=ToImageUrl(package.ImageUrl);
        }

        return Ok(business);
    }
    [HttpPost]
     [Authorize(Roles = UserRoles.Admin)]
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
        // clientul are nevoie de Id ca sa poata urca imaginea imediat dupa creare
        return CreatedAtAction(nameof(GetOneById), new { id = bus.Id }, new { bus.Id });
         }

    [HttpPost("{id}/image")]
    [Authorize(Roles = UserRoles.Admin)]
    public async Task<IActionResult> UploadImage(int id, IFormFile file)
    {
        var business = await _context.Businesses.FindAsync(id);
        if (business is null)
        {
            return NotFound();
        }
        string newPath;
        try
        {
            newPath = await _imageStorage.SaveAsync(file, "businesses");
        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
        _imageStorage.Delete(business.ImagePath);
        business.ImagePath = newPath;
        await _context.SaveChangesAsync();
        return Ok(new { ImageUrl = ToImageUrl(newPath) });
    }
    [HttpPut("{id}")]
     [Authorize(Roles = UserRoles.Admin)]
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
     [Authorize(Roles = UserRoles.Admin)]
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



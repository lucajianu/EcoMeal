using System.ComponentModel.DataAnnotations;

namespace EcoMeal.Site.Models;

public class BusinessAddModel
{
    [Required(ErrorMessage = "Numele este obligatoriu")]
    [StringLength(50)]
    public required string Name { get; set; }
    [Required(ErrorMessage = "Adresa este obligatorie")]
    [StringLength(100)]
    public required string Address { get; set; }
    public string? Description { get; set; }
    [Required(ErrorMessage = "Contactul este obligatoriu")]
    [StringLength(50)]
    public required string Contact { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Alege tipul de business")]
    public int BusinessTypeId { get; set; }
}

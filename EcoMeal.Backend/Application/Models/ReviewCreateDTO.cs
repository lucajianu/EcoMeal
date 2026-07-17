using System.ComponentModel.DataAnnotations;

namespace EcoMeal.Backend.Models;
public class ReviewCreateDTO
{
    [Required]
    public int OrderId { get; set; }
    [Range(1, 5, ErrorMessage = "Nota trebuie sa fie intre 1 si 5")]
    public int Rating { get; set; }
    [StringLength(500)]
    public string? Comment { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace EcoMeal.Backend.Models;
public  class OrderCreateDTO
{
    [Required]
    public int PackageId{get;set;}
    [Range(1, 100)]
    public int Count{get;set;} = 1;
}

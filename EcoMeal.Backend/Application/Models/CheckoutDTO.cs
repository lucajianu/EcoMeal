using System.ComponentModel.DataAnnotations;

namespace EcoMeal.Backend.Models;

// cosul trimis la checkout: fiecare item devine o comanda separata
public class CheckoutDTO
{
    [MinLength(1, ErrorMessage = "Cosul este gol")]
    public List<OrderCreateDTO> Items { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;
public class PackageAddModel
{
    [Required(ErrorMessage ="Numele este obligatoriu")]
    [StringLengthAttribute(50)]
    public required string Name { get; set; }
     [Required(ErrorMessage ="Descrierea este obligatorie")]
    [StringLengthAttribute(100)]
    public required string Description { get; set; }
    [Required]
    [Range(0,1000)]
        public double Price { get; set; }
    [Range(1, 1000, ErrorMessage = "Numarul de pachete trebuie sa fie cel putin 1")]
    public int NoPackage { get; set; } = 1;
        [Required]
    public DateTime StartPickup { get; set; }
    [Required]
    public DateTime EndPickup { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Alege tipul de pachet")]
    public int PackageTypeId { get; set; }
    public int BusinessId { get; set; }
}
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
        [Required]
    public DateTime StartPickup { get; set; }
    [Required]
    public DateTime EndPickup { get; set; }
    [Required]
    public int PackageTypeId { get; set; }
}
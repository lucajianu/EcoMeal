using System.ComponentModel.DataAnnotations;

public class BusinessType
{

    public int Id{get;set;}
    [MaxLength(20)]
    public required  string Name{get;set;}
    public  ICollection<Business> Businesses=new List<Business>();// IEnumerable/ICollection= IEnumerable+ metode noi
}
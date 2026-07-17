using EcoMeal.Backend.Entities;

public class OrderGetDTO
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public int PackageId{get;set;}
    public string PackageName {get;set;}="";
    public double Price{get;set;}
    public int Count{get;set;}
    public State Status{get;set;}
    public DateTime? Date{get;set;}
    public int BusinessId{get;set;}
    public string BusinessName{get;set;}="";
    public string UserName{get;set;}="";
    public string UserContact{get;set;}="";
    public int? ReviewRating{get;set;}
}

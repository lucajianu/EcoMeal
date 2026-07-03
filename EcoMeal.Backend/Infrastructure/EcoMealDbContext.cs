using EcoMeal.Backend.Entities;
using Microsoft.EntityFrameworkCore;

public class EcoMealDbContext: DbContext// baza noastra de date
{
  public EcoMealDbContext(DbContextOptions<EcoMealDbContext> options) : base(options)
    {   }
    DbSet<User> Users{get;set;}   //am creat tabelul users
}

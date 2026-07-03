using EcoMeal.Backend.Entities;
using Microsoft.EntityFrameworkCore;

public class EcoMealDbContext: DbContext// baza noastra de date
{
  public EcoMealDbContext(DbContextOptions<EcoMealDbContext> options) : base(options)
    {   }
    public DbSet<User> Users{get;set;}   //am creat tabelul users
      public DbSet<BusinessType> BusinessTypes{get;set;}
       public DbSet<PackageType> PackageTypes{get;set;}
       public DbSet<Business> Businesses{get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>()
        .HasKey(e=>e.Id);
        modelBuilder.Entity<Business>()
        .HasOne(p=>p.BusinessType)
        .WithMany(p=>p.Businesses)
        .HasForeignKey(p=>p.BusinessTypeId);

    }
}

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
       public DbSet<Package> Packages{get;set;}
       public DbSet<Order> Orders{get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    
        modelBuilder.Entity<Business>()
        .HasOne(b=>b.BusinessType)
        .WithMany(b=>b.Businesses)
        .HasForeignKey(b=>b.BusinessTypeId);

        modelBuilder.Entity<Order>()
        .HasOne(o=>o.User)
        .WithMany(u=>u.Orders)
        .HasForeignKey(o=>o.UserId);
        
        modelBuilder.Entity<Order>()
        .HasOne(o=>o.Package)
        .WithMany(o=>o.Orders)
        .HasForeignKey(o => o.PackageId);

        modelBuilder.Entity<Package>()
        .HasOne(p=>p.PackageType)
        .WithMany(p=>p.Packages)
        .HasForeignKey(p=>p.PackageTypeId);

        modelBuilder.Entity<Package>()
        .HasOne(p=>p.Business)
        .WithMany(p=>p.Packages)
        .HasForeignKey(p=>p.BusinessId);
         


    }
}

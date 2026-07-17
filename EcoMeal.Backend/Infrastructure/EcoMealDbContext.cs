using EcoMeal.Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class EcoMealDbContext: IdentityDbContext<User,IdentityRole<int>,int>// baza noastra de date
{
  public EcoMealDbContext(DbContextOptions<EcoMealDbContext> options) : base(options){}
    
      public DbSet<BusinessType> BusinessTypes{get;set;}
       public DbSet<PackageType> PackageTypes{get;set;}
       public DbSet<Business> Businesses{get;set;}
       public DbSet<Package> Packages{get;set;}
       public DbSet<Order> Orders{get;set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
    
        modelBuilder.Entity<Business>()
        .HasOne(b=>b.BusinessType)
        .WithMany(b=>b.Businesses)
        .HasForeignKey(b=>b.BusinessTypeId);

        // stocam enum-ul ca text ("Pending", "Accepted"...) ca sa fie lizibil in baza de date
        modelBuilder.Entity<Order>()
        .Property(o=>o.Status)
        .HasConversion<string>()
        .HasMaxLength(20);

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

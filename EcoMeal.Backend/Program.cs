using System.Text.Json.Serialization;
using EcoMeal.Backend.Application.Constants;
using EcoMeal.Backend.Entities;
using EcoMeal.Backend.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<EcoMealDbContext>(
    options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddIdentityApiEndpoints<User>(options =>
{ options.SignIn.RequireConfirmedAccount=false;
    options.Password.RequireNonAlphanumeric=false;

}).AddRoles<IdentityRole<int>>().AddEntityFrameworkStores<EcoMealDbContext>();

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    // originile CORS nu au voie sa aiba slash la final, altfel nu se potrivesc niciodata
    options.AddPolicy("AllowBlazorSite",policy=>{policy.WithOrigins("https://localhost:7109").AllowAnyHeader().AllowAnyMethod();
    });
});
// enum-urile se trimit ca text in JSON ("Pending"), nu ca numere (0)
builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddScoped<ImageStorage>();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options=> 
    options.SwaggerEndpoint("/openapi/v1.json","EcoMeal API"));
}
app.UseHttpsRedirection();
app.UseStaticFiles();// serveste imaginile din wwwroot
app.UseCors("AllowBlazorSite");
app.UseAuthentication();
app.UseAuthorization();
app.MapIdentityApi<User>();
app.MapControllers();
app.MapHub<EcoMeal.Backend.Application.Hubs.OrderHub>("/hubs/orders");
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var roles = new[] { UserRoles.Admin, UserRoles.User };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
        }
    }

    // seed the administrator account (credentials come from appsettings "SeedAdmin")
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var adminEmail = app.Configuration["SeedAdmin:Email"] ?? "admin@ecomeal.com";
    var adminPassword = app.Configuration["SeedAdmin:Password"] ?? "Admin123";

    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin is null)
    {
        admin = new User
        {
            UserName = adminEmail,
            Email = adminEmail,
            Name = "Administrator",
            Contact = adminEmail
        };
        var result = await userManager.CreateAsync(admin, adminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, UserRoles.Admin);
        }
    }
    else if (!await userManager.IsInRoleAsync(admin, UserRoles.Admin))
    {
        await userManager.AddToRoleAsync(admin, UserRoles.Admin);
    }
}

app.Run();

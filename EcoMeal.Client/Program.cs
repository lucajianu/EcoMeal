using EcoMeal.Client.Components;
using EcoMeal.Site.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider,CustomAuthenticationStateProvider>();
builder.Services.AddScoped(sp =>
{
    var innerHandler = new HttpClientHandler();

    // In development the backend uses the ASP.NET self-signed dev certificate;
    // accept it so server-to-server API calls don't fail TLS validation.
    if (builder.Environment.IsDevelopment())
    {
        innerHandler.ServerCertificateCustomValidationCallback =
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }

    var handler = new AuthenticationHeaderHandler(
        sp.GetRequiredService<ProtectedLocalStorage>(),
        sp.GetRequiredService<ILogger<AuthenticationHeaderHandler>>())
    {
        InnerHandler = innerHandler
    };
    return new HttpClient(handler) { BaseAddress = new Uri("https://localhost:7171/") };
});
builder.Services.AddScoped<EcoMeal.Site.Services.BusinessService>();
builder.Services.AddScoped<EcoMeal.Site.Services.PackageService>();
builder.Services.AddScoped<EcoMeal.Site.Services.SearchState>();
builder.Services.AddScoped<EcoMeal.Site.Services.OrderService>();
builder.Services.AddScoped<EcoMeal.Site.Services.OrderNotificationService>();
builder.Services.AddScoped<EcoMeal.Site.Services.CartState>();
builder.Services.AddScoped<EcoMeal.Site.Services.NotificationCenter>();
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

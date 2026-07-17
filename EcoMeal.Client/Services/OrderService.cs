using System.Net.Http.Headers;
using EcoMeal.Client.Models;
using EcoMeal.Site.Services;

namespace EcoMeal.Site.Services;
public class OrderService
{
    private readonly HttpClient _http;
    private readonly AuthService _authService;
    public OrderService(HttpClient http, AuthService authService)
    {
        _http = http;
        _authService = authService;
    }

    // trimite tot cosul; intoarce null la succes sau mesajul de eroare de la server
    public async Task<string?> CheckoutAsync(IEnumerable<EcoMeal.Site.Models.CartLineModel> lines)
    {
        var items = lines.Select(l => new { l.PackageId, l.Count }).ToList();
        var request = new HttpRequestMessage(HttpMethod.Post, "api/order/checkout")
        {
            Content = JsonContent.Create(new { Items = items })
        };
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Comanda nu a putut fi plasată." : error;
    }

    public async Task<List<OrderGetModel>> GetMyOrdersAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/order/my");
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<OrderGetModel>>();
        return orders ?? new List<OrderGetModel>();
    }

    public async Task<List<OrderGetModel>> GetBusinessOrdersAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "api/order/business");
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var orders = await response.Content.ReadFromJsonAsync<List<OrderGetModel>>();
        return orders ?? new List<OrderGetModel>();
    }

    public async Task<bool> UpdateStatusAsync(int orderId, string status)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, $"api/order/{orderId}/status")
        {
            Content = JsonContent.Create(new { Status = status })
        };
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        return response.IsSuccessStatusCode;
    }

    // intoarce null la succes sau mesajul de eroare de la server
    public async Task<string?> SubmitReviewAsync(int orderId, int rating, string? comment)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/review")
        {
            Content = JsonContent.Create(new { OrderId = orderId, Rating = rating, Comment = comment })
        };
        await AddAuthHeaderAsync(request);

        var response = await _http.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return null;
        }
        var error = await response.Content.ReadAsStringAsync();
        return string.IsNullOrWhiteSpace(error) ? "Recenzia nu a putut fi trimisă." : error;
    }

    private async Task AddAuthHeaderAsync(HttpRequestMessage request)
    {
        if (string.IsNullOrEmpty(_authService.Token))
        {
            await _authService.LoadTokenAsync();
        }

        if (!string.IsNullOrEmpty(_authService.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _authService.Token);
        }
    }
}

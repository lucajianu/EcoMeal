using EcoMeal.Site.Models;

namespace EcoMeal.Site.Services;
public class BusinessService// in sine este doar DTO-ul din backend
{
    private readonly HttpClient _http;
    public BusinessService(HttpClient http)
    {
        _http=http;
    }
    public async Task<List<BusinessModel>> GetAllAsync()
    {
        var businesses=await _http.GetFromJsonAsync<List<BusinessModel>>("/api/business");
        //il transforma din Json in lista
        //ne returneaza lista 
        return businesses ?? new List<BusinessModel>();
        
    }
    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/business/{id}");
        return response.IsSuccessStatusCode;
    }
    public async Task<BusinessDetailsModel?> GetOneById(int id)
    {
        var business= await _http.GetFromJsonAsync<BusinessDetailsModel>($"api/business/{id}");
        return business;
    } 
    public async Task<List<BusinessTypeModel>> GetBusinessTypesAsync()
    {
        var types = await _http.GetFromJsonAsync<List<BusinessTypeModel>>("api/businesstypes");
        return types ?? new List<BusinessTypeModel>();
    }
    public async Task AddBusinessAsync(BusinessAddModel business)
    {
        await _http.PostAsJsonAsync("api/business", business);
    }
    public async Task UpdateBusinessAsync(int id, BusinessAddModel business)
    {
        await _http.PutAsJsonAsync($"api/business/{id}", business);
    }
}
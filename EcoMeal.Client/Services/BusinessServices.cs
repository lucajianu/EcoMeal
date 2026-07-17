using EcoMeal.Site.Models;
using Microsoft.AspNetCore.Components.Authorization;

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
    public async Task<int?> AddBusinessAsync(BusinessAddModel business)
    {
        var response = await _http.PostAsJsonAsync("api/business", business);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var created = await response.Content.ReadFromJsonAsync<CreatedIdModel>();
        return created?.Id;
    }
    public async Task UpdateBusinessAsync(int id, BusinessAddModel business)
    {
        await _http.PutAsJsonAsync($"api/business/{id}", business);
    }
    public async Task<bool> UploadImageAsync(int id, Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await _http.PostAsync($"api/business/{id}/image", content);
        return response.IsSuccessStatusCode;
    }
}
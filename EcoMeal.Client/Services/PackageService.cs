using EcoMeal.Site.Models;

namespace EcoMeal.Site.Services;
public class PackageService
{
    private readonly HttpClient _http;
    public PackageService(HttpClient http)
    {
        _http = http;
    }
    public async Task<List<PackageTypeModel>> GetPackageTypesAsync()
    {
        var types = await _http.GetFromJsonAsync<List<PackageTypeModel>>("api/packagetypes");
        return types ?? new List<PackageTypeModel>();
    }
    public async Task<PackageGetModel?> GetPackageByIdAsync(int id)
    {
        var response = await _http.GetAsync($"api/packages/{id}");
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        return await response.Content.ReadFromJsonAsync<PackageGetModel>();
    }
    public async Task AddPackageAsync(PackageAddModel package)
    {
        await _http.PostAsJsonAsync("api/packages", package);
    }
    public async Task UpdatePackageAsync(int id, PackageAddModel package)
    {
        await _http.PutAsJsonAsync($"api/packages/{id}", package);
    }
    public async Task<bool> DeletePackageAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/packages/{id}");
        return response.IsSuccessStatusCode;
    }
}

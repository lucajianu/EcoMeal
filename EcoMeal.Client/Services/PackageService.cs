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
    public async Task<int?> AddPackageAsync(PackageAddModel package)
    {
        var response = await _http.PostAsJsonAsync("api/packages", package);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }
        var created = await response.Content.ReadFromJsonAsync<CreatedIdModel>();
        return created?.Id;
    }
    public async Task<bool> UploadImageAsync(int id, Microsoft.AspNetCore.Components.Forms.IBrowserFile file)
    {
        using var content = new MultipartFormDataContent();
        var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024));
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.Name);

        var response = await _http.PostAsync($"api/packages/{id}/image", content);
        return response.IsSuccessStatusCode;
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

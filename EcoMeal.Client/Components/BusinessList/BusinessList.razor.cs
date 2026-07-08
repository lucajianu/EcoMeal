using EcoMeal.Site.Models;
using EcoMeal.Site.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.Site.Components.BusinessList;

public partial class BusinessList
{
    [Inject]
    public required BusinessService BusinessService { get; set; }

    private List<BusinessModel>? Businesses { get; set; }

    private string SearchTerm { get; set; } = string.Empty;

    private string? SelectedType { get; set; }

    private IEnumerable<string> Types =>
        Businesses?.Select(b => b.BusinessTypeName).Distinct().OrderBy(t => t)
        ?? Enumerable.Empty<string>();

    private IEnumerable<BusinessModel> Filtered =>
        (Businesses ?? Enumerable.Empty<BusinessModel>())
            .Where(b => SelectedType is null || b.BusinessTypeName == SelectedType)
            .Where(b => string.IsNullOrWhiteSpace(SearchTerm)
                || b.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase));

    protected override async Task OnInitializedAsync()
    {
        Businesses = await BusinessService.GetAllAsync();
    }

    private void DeleteBusiness(int id)
    {
        Businesses = Businesses?.Where(b => b.Id != id).ToList();
    }
}

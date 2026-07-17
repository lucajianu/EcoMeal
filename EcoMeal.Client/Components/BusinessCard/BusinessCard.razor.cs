using EcoMeal.Site.Models;
using EcoMeal.Site.Services;
using Microsoft.AspNetCore.Components;

namespace EcoMeal.Site.Components.BusinessCard;

public partial class BusinessCard
{
    [Parameter]
    public required BusinessModel Business { get; set; }

    [Parameter]
    public EventCallback<int> OnDelete { get; set; }

    [Inject]
    public required BusinessService BusinessService { get; set; }
     [Inject]
     public required NavigationManager Navigation{get;set;}

    private string Monogram =>
        string.IsNullOrEmpty(Business.Name) ? "?" : Business.Name[..1].ToUpperInvariant();

    // Deterministic color per business type, so all "Fast Food" cards share an accent
    private string AccentClass
    {
        get
        {
            var sum = 0;
            foreach (var c in Business.BusinessTypeName)
            {
                sum += c;
            }
            return $"accent-{sum % 5}";
        }
    }
    
    private async Task DeleteBusiness()
    {
        var success = await BusinessService.DeleteAsync(Business.Id);
        if (success)
        {
            await OnDelete.InvokeAsync(Business.Id);
        }
    }
   
    public void NavigateToDetails()
    {
        Navigation.NavigateTo($"business/{Business.Id}");
    }

    public void NavigateToEdit()
    {
        Navigation.NavigateTo($"editBusiness/{Business.Id}");
    }
}
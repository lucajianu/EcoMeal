using EcoMeal.Site.Models;
using EcoMeal.Site.Services;
using Microsoft.AspNetCore.Components;
namespace EcoMeal.Site.Components.BusinessCard;
public partial class BusinessCard
{
    [Parameter]
    public required BusinessModel Business{get;set;}
    [Inject]
    public required BusinessService BusinessService{get;set;}
}
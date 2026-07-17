namespace EcoMeal.Site.Services;

/// <summary>
/// Shared state for the navbar search box. The MainLayout writes the term,
/// the BusinessList listens and re-filters. Scoped per circuit.
/// </summary>
public class SearchState
{
    public string Term { get; private set; } = string.Empty;

    public event Action? Changed;

    public void SetTerm(string term)
    {
        Term = term;
        Changed?.Invoke();
    }
}

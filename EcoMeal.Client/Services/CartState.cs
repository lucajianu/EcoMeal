using EcoMeal.Site.Models;

namespace EcoMeal.Site.Services;

// starea cosului, per circuit (acelasi pattern ca SearchState);
// navbar-ul si pagina de cos se aboneaza la OnChange
public class CartState
{
    private readonly List<CartLineModel> _lines = new();
    public IReadOnlyList<CartLineModel> Lines => _lines;

    public event Action? OnChange;

    public int TotalCount => _lines.Sum(l => l.Count);
    public double TotalPrice => _lines.Sum(l => l.LineTotal);

    // acelasi pachet adaugat din nou doar creste cantitatea liniei existente
    public void Add(CartLineModel line)
    {
        var existing = _lines.FirstOrDefault(l => l.PackageId == line.PackageId);
        if (existing is null)
        {
            _lines.Add(line);
        }
        else
        {
            existing.Count = Math.Min(existing.Count + line.Count, existing.Available);
        }
        OnChange?.Invoke();
    }

    public void SetCount(int packageId, int count)
    {
        var line = _lines.FirstOrDefault(l => l.PackageId == packageId);
        if (line is null)
        {
            return;
        }
        line.Count = Math.Clamp(count, 1, line.Available);
        OnChange?.Invoke();
    }

    public void Remove(int packageId)
    {
        _lines.RemoveAll(l => l.PackageId == packageId);
        OnChange?.Invoke();
    }

    public void Clear()
    {
        _lines.Clear();
        OnChange?.Invoke();
    }
}

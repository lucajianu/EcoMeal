namespace EcoMeal.Backend.Infrastructure;

public class ImageStorage
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxSizeBytes = 2 * 1024 * 1024;

    private readonly IWebHostEnvironment _env;
    public ImageStorage(IWebHostEnvironment env)
    {
        _env = env;
    }

    // salveaza fisierul in wwwroot/images/<subFolder> si intoarce calea relativa ("/images/...")
    // arunca ArgumentException daca fisierul nu e o imagine valida
    public async Task<string> SaveAsync(IFormFile file, string subFolder)
    {
        if (file.Length == 0)
        {
            throw new ArgumentException("Fisierul este gol");
        }
        if (file.Length > MaxSizeBytes)
        {
            throw new ArgumentException("Imaginea poate avea maxim 2 MB");
        }
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Sunt acceptate doar imagini .jpg, .png sau .webp");
        }

        var folder = Path.Combine(WebRoot, "images", subFolder);
        Directory.CreateDirectory(folder);

        // nume generat, ca fisierele sa nu se suprascrie intre ele
        var fileName = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(folder, fileName);
        await using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }
        return $"/images/{subFolder}/{fileName}";
    }

    public void Delete(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath))
        {
            return;
        }
        var fullPath = Path.Combine(WebRoot, relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    // WebRootPath e null daca folderul wwwroot nu exista inca la pornire
    private string WebRoot => _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
}

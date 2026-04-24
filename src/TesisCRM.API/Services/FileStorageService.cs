namespace TesisCRM.API.Services;

public class FileStorageService
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    public FileStorageService(IWebHostEnvironment env, IConfiguration config) { _env = env; _config = config; }

    public async Task<string> SaveAsync(IFormFile file, string folder)
    {
        var root = Path.Combine(_env.ContentRootPath, _config["Storage:RootPath"] ?? "Storage", folder);
        Directory.CreateDirectory(root);
        var ext = Path.GetExtension(file.FileName);
        var name = $"{Guid.NewGuid():N}{ext}";
        var full = Path.Combine(root, name);
        await using var fs = new FileStream(full, FileMode.Create);
        await file.CopyToAsync(fs);
        return full;
    }
}

using Microsoft.Extensions.Options;

namespace CareerCraft.Web.Models;

public sealed class ServiceOffering
{
    public string Slug { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public string Summary { get; set; } = "";
    public string Details { get; set; } = "";
    public string Timeline { get; set; } = "";
    public string Accent { get; set; } = "";
}

public sealed class StudioSettings
{
    public string BusinessName { get; set; } = "CareerCraft Studio";
    public string ContactEmail { get; set; } = "";
    public string Currency { get; set; } = "ZAR";
    public List<ServiceOffering> Services { get; set; } = [];
}

public sealed class ServiceCatalogue(IOptions<StudioSettings> settings)
{
    private readonly StudioSettings _settings = settings.Value;
    public string ContactEmail => _settings.ContactEmail;
    public IReadOnlyList<ServiceOffering> All => _settings.Services.AsReadOnly();
    public ServiceOffering? Find(string slug) =>
        _settings.Services.FirstOrDefault(s => string.Equals(s.Slug, slug, StringComparison.OrdinalIgnoreCase));
}

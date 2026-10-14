using CareerCraft.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace CareerCraft.Web.Controllers;

public class HomeController(ServiceCatalogue catalogue) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(catalogue.All);

    [HttpGet]
    public IActionResult Services() => View(catalogue.All);

    [HttpGet]
    public IActionResult Enquiry(string? service)
    {
        var selection = service is not null && catalogue.Find(service) is not null ? service : "";
        return View("Request", new EnquiryViewModel { ServiceSlug = selection, Services = catalogue.All });
    }

    // Phase 1 intentionally avoids receiving/storing a completed enquiry at the server:
    // The customer opens an email draft locally rather than uploading sensitive information.
    // The later authenticated backend will introduce secure request submission.
    [HttpGet]
    public IActionResult Privacy() => View();

    [HttpGet]
    public IActionResult Error() => View();
}

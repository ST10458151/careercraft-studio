using System.ComponentModel.DataAnnotations;

namespace CareerCraft.Web.Models;

public class EnquiryViewModel
{
    [Required(ErrorMessage = "Choose a service to get started.")]
    [Display(Name = "Choose your service")]
    public string ServiceSlug { get; set; } = "";

    [Required, StringLength(100)]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required, EmailAddress, StringLength(200)]
    [Display(Name = "Email address")]
    public string Email { get; set; } = "";

    [Required, StringLength(180)]
    [Display(Name = "Target job title")]
    public string TargetRole { get; set; } = "";

    [StringLength(250)]
    [Display(Name = "Specific vacancy or job link")]
    public string? JobLink { get; set; }

    [StringLength(1200)]
    [Display(Name = "Tell us what you need")]
    public string? Goals { get; set; }

    [Required(ErrorMessage = "Please acknowledge the enquiry terms.")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "Please acknowledge the enquiry terms.")]
    public bool Acknowledge { get; set; }

    public IReadOnlyList<ServiceOffering> Services { get; set; } = [];
}

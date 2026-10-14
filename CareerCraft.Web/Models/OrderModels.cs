using System.ComponentModel.DataAnnotations;

namespace CareerCraft.Web.Models;
public enum OrderStage { Submitted, InReview, InProgress, ReadyForReview, Completed, Cancelled }
public enum DocumentKind { ExistingCv, CoverLetter, Qualifications, JobDescription, Other, FinalDocument }
public sealed class CvOrder
{
    public int Id { get; set; }
    [Required] public string CustomerId { get; set; } = "";
    [Required, MaxLength(100)] public string ServiceName { get; set; } = "";
    [Required, MaxLength(60)] public string ServiceSlug { get; set; } = "";
    public decimal PriceZar { get; set; }
    [Required, MaxLength(160)] public string TargetRole { get; set; } = "";
    [MaxLength(140)] public string? Industry { get; set; }
    [MaxLength(2000)] public string? Education { get; set; }
    [MaxLength(4000)] public string? Experience { get; set; }
    [MaxLength(2000)] public string? Achievements { get; set; }
    [MaxLength(2000)] public string? Skills { get; set; }
    [MaxLength(2000)] public string? CareerGoals { get; set; }
    [MaxLength(1000)] public string? JobLinks { get; set; }
    [MaxLength(4000)] public string? JobDescription { get; set; }
    public OrderStage Stage { get; set; } = OrderStage.Submitted;
    public bool IsPaid { get; set; } // Always false until a verified payment webhook is implemented.
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public List<OrderDocument> Documents { get; set; } = [];
    public List<OrderRevision> Revisions { get; set; } = [];
}
public sealed class OrderDocument
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public CvOrder Order { get; set; } = null!;
    [Required,MaxLength(250)] public string OriginalName { get; set; } = "";
    [Required,MaxLength(100)] public string StorageName { get; set; } = "";
    [Required,MaxLength(120)] public string ContentType { get; set; } = "";
    public long SizeBytes { get; set; }
    public DocumentKind Kind { get; set; }
    public bool UploadedByAdmin { get; set; }
    public DateTime UploadedUtc { get; set; } = DateTime.UtcNow;
}
public sealed class OrderRevision
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public CvOrder Order { get; set; } = null!;
    [Required,MaxLength(2000)] public string Message { get; set; } = "";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
public sealed class OrderForm
{
    [Required] public string ServiceSlug { get; set; } = "";
    [Required,StringLength(160)] public string TargetRole { get; set; } = "";
    [StringLength(140)] public string? Industry { get; set; }
    [StringLength(2000)] public string? Education { get; set; }
    [StringLength(4000)] public string? Experience { get; set; }
    [StringLength(2000)] public string? Achievements { get; set; }
    [StringLength(2000)] public string? Skills { get; set; }
    [StringLength(2000)] public string? CareerGoals { get; set; }
    [StringLength(1000)] public string? JobLinks { get; set; }
    [StringLength(4000)] public string? JobDescription { get; set; }
    public IReadOnlyList<ServiceOffering> Services { get; set; } = [];
}

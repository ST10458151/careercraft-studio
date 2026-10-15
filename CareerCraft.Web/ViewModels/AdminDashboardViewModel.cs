using CareerCraft.Web.Models;

namespace CareerCraft.Web.ViewModels;

public sealed class AdminDashboardViewModel
{
    public string Search { get; set; } = "";
    public string Filter { get; set; } = "all";
    public int Total { get; set; }
    public int New { get; set; }
    public int InProgress { get; set; }
    public int AwaitingClient { get; set; }
    public int Completed { get; set; }
    public int WithRevisions { get; set; }
    public List<AdminOrderRow> Orders { get; set; } = [];
}

public sealed class AdminOrderRow
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public string ServiceName { get; set; } = "";
    public string TargetRole { get; set; } = "";
    public decimal PriceZar { get; set; }
    public DateTime CreatedUtc { get; set; }
    public OrderStage Stage { get; set; }
    public bool IsPaid { get; set; }
    public int DocumentCount { get; set; }
    public int RevisionCount { get; set; }
    public string StatusLabel => Stage switch
    {
        OrderStage.Submitted => "New request",
        OrderStage.InReview => "Reviewing details",
        OrderStage.InProgress => "CV being prepared",
        OrderStage.ReadyForReview => "Awaiting client review",
        OrderStage.Completed => "Completed",
        OrderStage.Cancelled => "Cancelled",
        _ => Stage.ToString()
    };
    public string StatusClass => Stage switch
    {
        OrderStage.Submitted => "new",
        OrderStage.InReview => "review",
        OrderStage.InProgress => "progress",
        OrderStage.ReadyForReview => "waiting",
        OrderStage.Completed => "done",
        _ => "cancelled"
    };
}

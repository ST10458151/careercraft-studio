using CareerCraft.Web.Data;
using CareerCraft.Web.Models;
using CareerCraft.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCraft.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(StudioDbContext db) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(string? search, string filter = "all")
    {
        var validFilters = new[] { "all", "new", "active", "review", "completed" };
        if (!validFilters.Contains(filter)) filter = "all";
        search = search?.Trim() ?? "";

        var counts = await db.Orders.AsNoTracking()
            .GroupBy(x => x.Stage)
            .Select(g => new { Stage = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Stage, x => x.Count);
        int Count(OrderStage stage) => counts.GetValueOrDefault(stage, 0);

        var orders = db.Orders.AsNoTracking().AsQueryable();
        if (!string.IsNullOrEmpty(search))
        {
            orders = orders.Where(x =>
                x.TargetRole.Contains(search) ||
                x.ServiceName.Contains(search) ||
                db.Users.Any(u => u.Id == x.CustomerId && u.Email != null && u.Email.Contains(search)));
        }
        orders = filter switch
        {
            "new" => orders.Where(x => x.Stage == OrderStage.Submitted),
            "active" => orders.Where(x => x.Stage == OrderStage.InReview || x.Stage == OrderStage.InProgress),
            "review" => orders.Where(x => x.Stage == OrderStage.ReadyForReview),
            "completed" => orders.Where(x => x.Stage == OrderStage.Completed),
            _ => orders
        };
        var rows = await (
            from order in orders
            join user in db.Users.AsNoTracking() on order.CustomerId equals user.Id
            orderby order.CreatedUtc descending
            select new AdminOrderRow
            {
                Id = order.Id,
                CustomerEmail = user.Email ?? "Unknown",
                ServiceName = order.ServiceName,
                TargetRole = order.TargetRole,
                PriceZar = order.PriceZar,
                CreatedUtc = order.CreatedUtc,
                Stage = order.Stage,
                IsPaid = order.IsPaid,
                DocumentCount = order.Documents.Count,
                RevisionCount = order.Revisions.Count
            }).Take(150).ToListAsync();

        var vm = new AdminDashboardViewModel
        {
            Search = search,
            Filter = filter,
            Total = counts.Values.Sum(),
            New = Count(OrderStage.Submitted),
            InProgress = Count(OrderStage.InReview) + Count(OrderStage.InProgress),
            AwaitingClient = Count(OrderStage.ReadyForReview),
            Completed = Count(OrderStage.Completed),
            WithRevisions = await db.Orders.CountAsync(x => x.Revisions.Any()),
            Orders = rows
        };
        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var order = await db.Orders.AsNoTracking().Include(x => x.Documents).Include(x => x.Revisions).FirstOrDefaultAsync(x => x.Id == id);
        return order == null ? NotFound() : View(order);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id, OrderStage stage, string? returnFilter, string? returnSearch, bool returnToCase = false)
    {
        var order = await db.Orders.FindAsync(id);
        if (order == null) return NotFound();
        if (!Enum.IsDefined(stage)) return BadRequest();
        if (order.Stage == OrderStage.Cancelled && stage != OrderStage.Cancelled)
        {
            TempData["Error"] = "Cancelled requests cannot be reopened from this dashboard.";
            return RedirectToAction(nameof(Index), new { filter = returnFilter, search = returnSearch });
        }
        order.Stage = stage;
        await db.SaveChangesAsync();
        TempData["Success"] = $"Request #{order.Id:D5} updated to {stage}.";
        return returnToCase ? RedirectToAction(nameof(Details),new { id }) : RedirectToAction(nameof(Index), new { filter = returnFilter, search = returnSearch });
    }
}

using CareerCraft.Web.Data;
using CareerCraft.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace CareerCraft.Web.Controllers;

[Authorize(Roles="Admin")]
public class AdminController(StudioDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search)
    {
        var orders=db.Orders.AsNoTracking().Include(x=>x.Documents).Include(x=>x.Revisions).AsQueryable();
        if(!string.IsNullOrWhiteSpace(search))orders=orders.Where(x=>x.TargetRole.Contains(search)||x.ServiceName.Contains(search));
        ViewBag.Total=await db.Orders.CountAsync();
        ViewBag.Active=await db.Orders.CountAsync(x=>x.Stage!=OrderStage.Completed&&x.Stage!=OrderStage.Cancelled);
        ViewBag.Unpaid=await db.Orders.CountAsync(x=>!x.IsPaid);
        return View(await orders.OrderByDescending(x=>x.CreatedUtc).ToListAsync());
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeStatus(int id,OrderStage stage)
    {
        var order=await db.Orders.FindAsync(id);
        if(order==null)return NotFound();
        if(!Enum.IsDefined(stage))return BadRequest();
        order.Stage=stage;await db.SaveChangesAsync();
        TempData["Success"]="Order status updated.";
        return RedirectToAction(nameof(Index));
    }
}

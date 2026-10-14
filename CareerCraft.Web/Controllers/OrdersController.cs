using System.Security.Claims;
using CareerCraft.Web.Data;
using CareerCraft.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCraft.Web.Controllers;
[Authorize]
public class OrdersController(StudioDbContext db,ServiceCatalogue catalogue) : Controller
{
    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders=await db.Orders.AsNoTracking().Where(x=>x.CustomerId==UserId)
            .OrderByDescending(x=>x.CreatedUtc).ToListAsync();
        return View(orders);
    }
    [HttpGet]
    public IActionResult Create(string? service=null)=>View(new OrderForm{ServiceSlug=catalogue.Find(service??"")?.Slug??"",Services=catalogue.All});
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderForm form)
    {
        var selected=catalogue.Find(form.ServiceSlug);
        if(selected==null)ModelState.AddModelError(nameof(form.ServiceSlug),"Choose a valid service.");
        if(!ModelState.IsValid){form.Services=catalogue.All;return View(form);}
        var item=new CvOrder{
          CustomerId=UserId,ServiceSlug=selected!.Slug,ServiceName=selected.Name,PriceZar=selected.Price,
          TargetRole=form.TargetRole.Trim(),Industry=form.Industry,Education=form.Education,
          Experience=form.Experience,Achievements=form.Achievements,Skills=form.Skills,
          CareerGoals=form.CareerGoals,JobLinks=form.JobLinks,JobDescription=form.JobDescription
        };
        db.Orders.Add(item);await db.SaveChangesAsync();
        TempData["Success"]="Your request was saved. You can now attach your existing CV and other documents.";
        return RedirectToAction(nameof(Details),new{id=item.Id});
    }
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var order=await db.Orders.AsNoTracking().Include(x=>x.Documents).Include(x=>x.Revisions)
            .FirstOrDefaultAsync(x=>x.Id==id&&(x.CustomerId==UserId||User.IsInRole("Admin")));
        return order==null?NotFound():View(order);
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Revision(int id,[FromForm,StringLength(2000)] string message)
    {
        var order=await db.Orders.FirstOrDefaultAsync(x=>x.Id==id&&x.CustomerId==UserId);
        if(order==null)return NotFound();
        if(order.Stage!=OrderStage.ReadyForReview){TempData["Error"]="Revisions can be requested when the document is ready for review.";return RedirectToAction(nameof(Details),new{id});}
        if(string.IsNullOrWhiteSpace(message)){TempData["Error"]="Enter your revision request.";return RedirectToAction(nameof(Details),new{id});}
        db.Revisions.Add(new OrderRevision{OrderId=id,Message=message.Trim()});
        await db.SaveChangesAsync();TempData["Success"]="Revision request recorded.";
        return RedirectToAction(nameof(Details),new{id});
    }
}

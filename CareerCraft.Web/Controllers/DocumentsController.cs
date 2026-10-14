using System.Security.Claims;
using CareerCraft.Web.Data;
using CareerCraft.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareerCraft.Web.Controllers;
[Authorize]
public class DocumentsController(StudioDbContext db,IWebHostEnvironment environment) : Controller
{
    private const long MaxFileBytes=8*1024*1024;
    private string UserId=>User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    // Outside wwwroot, therefore never served by StaticFiles middleware.
    private string Root => Path.GetFullPath(Path.Combine(environment.ContentRootPath,"..","CareerCraftPrivateFiles"));
    [HttpGet]
    public async Task<IActionResult> Index(int? orderId)
    {
        var docs=await db.Documents.AsNoTracking().Include(x=>x.Order)
            .Where(x=>x.Order.CustomerId==UserId&&(orderId==null||x.OrderId==orderId))
            .OrderByDescending(x=>x.UploadedUtc).ToListAsync();
        ViewBag.OrderId=orderId;
        return View(docs);
    }
    [HttpPost,ValidateAntiForgeryToken,RequestSizeLimit(9*1024*1024)]
    public async Task<IActionResult> Upload(int orderId,DocumentKind kind,IFormFile? file)
    {
        var order=await db.Orders.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==orderId&&(x.CustomerId==UserId||User.IsInRole("Admin")));
        if(order==null)return NotFound();
        if(file==null||file.Length==0||file.Length>MaxFileBytes){TempData["Error"]="Select a PDF or DOCX file up to 8 MB.";return RedirectToAction("Details","Orders",new{id=orderId});}
        string extension=Path.GetExtension(file.FileName).ToLowerInvariant();
        if(extension!=".pdf"&&extension!=".docx"){TempData["Error"]="Only PDF and DOCX files are supported.";return RedirectToAction("Details","Orders",new{id=orderId});}
        if(kind==DocumentKind.FinalDocument&&!User.IsInRole("Admin"))return Forbid();
        await using var stream=file.OpenReadStream();
        var header=new byte[5];int count=await stream.ReadAsync(header.AsMemory());
        var signatureOk=extension==".pdf"?(count>=4&&header[0]==37&&header[1]==80&&header[2]==68&&header[3]==70):
            (count>=4&&header[0]==80&&header[1]==75&&header[2]==3&&header[3]==4);
        if(!signatureOk){TempData["Error"]="The file content doesn't match a supported PDF or DOCX.";return RedirectToAction("Details","Orders",new{id=orderId});}
        // Signature checks are not malware scanning. Production needs scanning and stricter DOCX inspection.
        stream.Position=0;
        Directory.CreateDirectory(Root);
        var stored=Guid.NewGuid().ToString("N")+extension;
        var fullPath=Path.Combine(Root,stored);
        try
        {
            await using var output=new FileStream(fullPath,FileMode.CreateNew,FileAccess.Write,FileShare.None,65536,FileOptions.Asynchronous);
            await stream.CopyToAsync(output);
            db.Documents.Add(new OrderDocument{
                OrderId=orderId,OriginalName=Path.GetFileName(file.FileName).Length>240?Path.GetFileName(file.FileName)[..240]:Path.GetFileName(file.FileName),
                StorageName=stored,ContentType=extension==".pdf"?"application/pdf":"application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                SizeBytes=file.Length,Kind=kind,UploadedByAdmin=User.IsInRole("Admin")
            });
            await db.SaveChangesAsync();
        }
        catch {System.IO.File.Delete(fullPath);throw;}
        TempData["Success"]="Document uploaded to the private local file area.";
        return RedirectToAction("Details","Orders",new{id=orderId});
    }
    [HttpGet]
    public async Task<IActionResult> Download(int id)
    {
        var doc=await db.Documents.AsNoTracking().Include(x=>x.Order).FirstOrDefaultAsync(x=>x.Id==id);
        if(doc==null)return NotFound();
        if(doc.Order.CustomerId!=UserId&&!User.IsInRole("Admin"))return Forbid();
        var file=Path.Combine(Root,Path.GetFileName(doc.StorageName));
        if(!System.IO.File.Exists(file))return NotFound();
        return PhysicalFile(file,doc.ContentType,doc.OriginalName);
    }
    [HttpPost,ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var doc=await db.Documents.Include(x=>x.Order).FirstOrDefaultAsync(x=>x.Id==id);
        if(doc==null)return NotFound();
        if(doc.Order.CustomerId!=UserId&&!User.IsInRole("Admin"))return Forbid();
        if(doc.UploadedByAdmin&&!User.IsInRole("Admin"))return Forbid();
        db.Documents.Remove(doc);await db.SaveChangesAsync();
        System.IO.File.Delete(Path.Combine(Root,Path.GetFileName(doc.StorageName)));
        TempData["Success"]="Document removed.";
        return RedirectToAction("Details","Orders",new{id=doc.OrderId});
    }
}

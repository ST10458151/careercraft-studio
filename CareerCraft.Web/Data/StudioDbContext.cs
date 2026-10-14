using CareerCraft.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareerCraft.Web.Data;
public sealed class StudioDbContext(DbContextOptions<StudioDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<CvOrder> Orders => Set<CvOrder>();
    public DbSet<OrderDocument> Documents => Set<OrderDocument>();
    public DbSet<OrderRevision> Revisions => Set<OrderRevision>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<CvOrder>().HasIndex(x=>x.CustomerId);
        builder.Entity<CvOrder>().Property(x=>x.PriceZar).HasPrecision(12,2);
        builder.Entity<OrderDocument>().HasOne(x=>x.Order).WithMany(x=>x.Documents).HasForeignKey(x=>x.OrderId).OnDelete(DeleteBehavior.Cascade);
        builder.Entity<OrderRevision>().HasOne(x=>x.Order).WithMany(x=>x.Revisions).HasForeignKey(x=>x.OrderId).OnDelete(DeleteBehavior.Cascade);
    }
}

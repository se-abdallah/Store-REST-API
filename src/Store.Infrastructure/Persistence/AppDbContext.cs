using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Domain.Entities;
using Store.Infrastructure.Identity;

namespace Store.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<
        AppUser,
        AppRole,
        int,
        IdentityUserClaim<int>,
        AppUserRole,
        IdentityUserLogin<int>,
        IdentityRoleClaim<int>,
        IdentityUserToken<int>>
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
  {
  }

  public DbSet<Product> Products { get; set; }
  public DbSet<ProductTranslation> ProductTranslations { get; set; }
  public DbSet<Invoice> Invoices { get; set; }
  public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
  public DbSet<Basket> Baskets { get; set; }
  public DbSet<BasketItem> BasketItems { get; set; }

  protected override void OnModelCreating(ModelBuilder builder)
  {
    base.OnModelCreating(builder);

    // AppUser 
    builder.Entity<AppUser>(user =>
    {
      user.HasMany(u => u.UserRoles)
                  .WithOne(ur => ur.User)
                  .HasForeignKey(ur => ur.UserId)
                  .IsRequired();
    });

    // AppRole 
    builder.Entity<AppRole>(role =>
    {
      role.HasMany(r => r.UserRoles)
                  .WithOne(ur => ur.Role)
                  .HasForeignKey(ur => ur.RoleId)
                  .IsRequired();
    });

    // AppUserRole
    builder.Entity<AppUserRole>(userRole =>
    {
      userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
    });

    // Product
    builder.Entity<Product>(entity =>
    {
      entity.Property(p => p.Price)
                    .HasColumnType("decimal(18,2)");

      entity.Property(p => p.StockQuantity)
                    .IsRequired();

      entity.HasMany(p => p.Translations)
                    .WithOne(t => t.Product)
                    .HasForeignKey(t => t.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);
    });

    // ProductTranslation
    builder.Entity<ProductTranslation>(entity =>
    {
      entity.Property(t => t.Language)
                    .HasMaxLength(5)
                    .IsRequired();

      entity.Property(t => t.Name)
                    .HasMaxLength(256)
                    .IsRequired();

      entity.HasIndex(t => new { t.ProductId, t.Language })
                    .IsUnique();
    });

    builder.Entity<Invoice>(entity =>
{
  entity.Property(i => i.TotalAmount)
            .HasColumnType("decimal(18,2)");

  entity.Property(i => i.CreatedAtUtc)
            .IsRequired();

  entity.Property(i => i.UserId)
            .IsRequired();
});

    builder.Entity<InvoiceDetail>(entity =>
    {
      entity.Property(d => d.UnitPrice)
          .HasColumnType("decimal(18,2)");

      entity.Property(d => d.LineTotal)
          .HasColumnType("decimal(18,2)");

      entity.Property(d => d.ProductName)
          .HasMaxLength(256)
          .IsRequired();

      entity.HasOne(d => d.Invoice)
          .WithMany(i => i.Details)
          .HasForeignKey(d => d.InvoiceId)
          .OnDelete(DeleteBehavior.Cascade);

      entity.HasOne(d => d.Product)
          .WithMany()
          .HasForeignKey(d => d.ProductId)
          .OnDelete(DeleteBehavior.Restrict);
    });

    // Basket
    builder.Entity<Basket>(entity =>
                {
                  entity.Property(b => b.UserId)
                    .IsRequired();

                  entity.HasMany(b => b.Items)
                    .WithOne(i => i.Basket)
                    .HasForeignKey(i => i.BasketId)
                    .OnDelete(DeleteBehavior.Cascade);
                });

    // BasketItem
    builder.Entity<BasketItem>(entity =>
    {
      entity.HasOne(i => i.Product)
                    .WithMany()
                    .HasForeignKey(i => i.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

      entity.Property(i => i.Quantity)
                    .IsRequired();
    });
  }
}
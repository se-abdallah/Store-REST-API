using Microsoft.EntityFrameworkCore;
using Store.Application.Params;
using Store.Domain.Entities;
using Store.Infrastructure.Persistence;
using Store.Infrastructure.Repositories;

namespace Store.Infrastructure.Tests;

public class InvoiceRepositoryTests
{
    private AppDbContext CreateInMemoryContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    /// Helper to seed one user with two invoices
    /// with different dates and totals.
    /// </summary>
    private async Task SeedInvoicesAsync(AppDbContext context, int userId)
    {

        var invoice1 = new Invoice
        {
            UserId = userId,
            CreatedAtUtc = new DateTime(2025, 01, 01, 10, 0, 0, DateTimeKind.Utc),
            TotalAmount = 50m,
            Details = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        ProductId = 1,
                        ProductName = "Pen",
                        UnitPrice = 10m,
                        Quantity = 5,
                        LineTotal = 50m
                    }
                }
        };

        var invoice2 = new Invoice
        {
            UserId = userId,
            CreatedAtUtc = new DateTime(2025, 01, 02, 12, 0, 0, DateTimeKind.Utc),
            TotalAmount = 120m,
            Details = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        ProductId = 2,
                        ProductName = "Notebook",
                        UnitPrice = 20m,
                        Quantity = 6,
                        LineTotal = 120m
                    }
                }
        };

        await context.Invoices.AddRangeAsync(invoice1, invoice2);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetInvoicesForUserAsync_Should_ReturnOnlyUserInvoices_AndRespectDateFilter()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        var invoiceRepository = new InvoiceRepository(context);

        var userId = 10;
        var otherUserId = 20;

        await SeedInvoicesAsync(context, userId);

        var otherInvoice = new Invoice
        {
            UserId = otherUserId,
            CreatedAtUtc = new DateTime(2025, 01, 03, 9, 0, 0, DateTimeKind.Utc),
            TotalAmount = 200m,
            Details = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        ProductId = 3,
                        ProductName = "Bag",
                        UnitPrice = 200m,
                        Quantity = 1,
                        LineTotal = 200m
                    }
                }
        };

        await context.Invoices.AddAsync(otherInvoice);
        await context.SaveChangesAsync();

        var @params = new InvoiceParams
        {
            PageNumber = 1,
            PageSize = 10,
            From = new DateTime(2025, 01, 01),
            To = new DateTime(2025, 01, 02, 23, 59, 59)
        };

        // Act
        var paged = await invoiceRepository.GetInvoicesForUserAsync(userId, @params);

        // Assert
        Assert.Equal(2, paged.TotalCount);
        Assert.All(paged, i => Assert.Equal(userId, i.UserId));

        Assert.All(paged, i =>
            Assert.InRange(
                i.CreatedAtUtc,
                @params.From.Value.ToUniversalTime(),
                @params.To.Value.ToUniversalTime()
            ));
    }

    [Fact]
    public async Task GetInvoicesForUserAsync_Should_SupportOrderingByAmountDesc_WithSqliteWorkaroundLogic()
    {
        // Arrange
        var dbName = Guid.NewGuid().ToString();
        using var context = CreateInMemoryContext(dbName);

        var invoiceRepository = new InvoiceRepository(context);
        var userId = 30;

        var smaller = new Invoice
        {
            UserId = userId,
            CreatedAtUtc = new DateTime(2025, 01, 01, 10, 0, 0, DateTimeKind.Utc),
            TotalAmount = 40m,
            Details = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        ProductId = 1,
                        ProductName = "Cheap item",
                        UnitPrice = 20m,
                        Quantity = 2,
                        LineTotal = 40m
                    }
                }
        };

        var bigger = new Invoice
        {
            UserId = userId,
            CreatedAtUtc = new DateTime(2025, 01, 01, 11, 0, 0, DateTimeKind.Utc),
            TotalAmount = 150m,
            Details = new List<InvoiceDetail>
                {
                    new InvoiceDetail
                    {
                        ProductId = 2,
                        ProductName = "Expensive item",
                        UnitPrice = 50m,
                        Quantity = 3,
                        LineTotal = 150m
                    }
                }
        };

        await context.Invoices.AddRangeAsync(smaller, bigger);
        await context.SaveChangesAsync();

        var @params = new InvoiceParams
        {
            PageNumber = 1,
            PageSize = 10,
            OrderBy = "amount_desc"
        };

        // Act
        var paged = await invoiceRepository.GetInvoicesForUserAsync(userId, @params);

        // Assert
        Assert.Equal(2, paged.TotalCount);
        Assert.Equal(150m, paged[0].TotalAmount);
        Assert.Equal(40m, paged[1].TotalAmount);
    }
}

using Store.Application.Pagination;

namespace Store.Application.Tests;

public class PagedListTests
{
    [Fact]
    public void PagedList_ShouldCalculatePaginationFieldsCorrectly()
    {
        // Arrange
        var source = Enumerable.Range(1, 50).ToList();
        int pageNumber = 2;
        int pageSize = 10;

        var pageItems = source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Act
        var paged = new PagedList<int>(pageItems, source.Count, pageNumber, pageSize);

        // Assert
        Assert.Equal(pageNumber, paged.CurrentPage);
        Assert.Equal(pageSize, paged.PageSize);
        Assert.Equal(source.Count, paged.TotalCount);
        Assert.Equal(5, paged.TotalPages);

        Assert.Equal(Enumerable.Range(11, 10), paged);
    }
}
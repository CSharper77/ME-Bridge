using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Invoice
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public List<LineItem> Items { get; set; } = new();
}

public class LineItem
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

[Map<Invoice>]
public class InvoiceModel
{
    public int Id { get; set; }
    public string Number { get; set; } = null!;
    public List<LineItemModel> Items { get; set; } = new();
}

[Map<LineItem>]
public class LineItemModel
{
    public int Id { get; set; }
    public string ProductName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class ModelToEntitySeeded_Tests
{
    [Fact]
    public async Task Map_ModelToEntity_ExistingByPk_ReturnsExisting()
    {
        using var ctx = EfTestHelpers.CreateSeededFullContext();
        var mapper = TestHelpers.CreateMapper();

        var model = new PersonModel
        {
            Id = 1,
            FirstName = "UpdatedA",
            LastName = "Smith",
            Age = 31,
            IsActive = true,
            Salary = 55000m,
            CreatedAt = new DateTime(2024, 6, 1)
        };

        var result = await mapper.BridgeTo<Person>(model, ctx);

        Assert.Equal(1, result.Id);
        Assert.Equal("UpdatedA", result.FirstName);
        Assert.Equal(31, result.Age);
        Assert.Equal(55000m, result.Salary);
    }

    [Fact]
    public async Task Map_ModelToEntity_NewByPk_CreatesNew()
    {
        using var ctx = EfTestHelpers.CreateSeededFullContext();
        var mapper = TestHelpers.CreateMapper();

        var model = new PersonModel
        {
            Id = 99,
            FirstName = "NewPerson",
            LastName = "Test",
            Age = 40,
            IsActive = true,
            Salary = 70000m,
            CreatedAt = new DateTime(2025, 1, 1)
        };

        var result = await mapper.BridgeTo<Person>(model, ctx);

        Assert.Equal(99, result.Id);
        Assert.Equal("NewPerson", result.FirstName);
    }

    [Fact]
    public async Task Map_ModelToEntity_WithNestedCollection_ExistingItemsLookedUp()
    {
        using var ctx = EfTestHelpers.CreateSeededFullContext();
        var mapper = TestHelpers.CreateMapper();

        var model = new InvoiceModel
        {
            Id = 10,
            Number = "INV-001-UPDATED",
            Items = new List<LineItemModel>
            {
                new() { Id = 100, ProductName = "Widget PRO", Quantity = 3, UnitPrice = 14.99m },
                new() { Id = 6, ProductName = "Updated", Quantity = 10 }
            }
        };

        var result = await mapper.BridgeTo<Invoice>(model, ctx);

        Assert.Equal(10, result.Id);
        Assert.Equal("INV-001-UPDATED", result.Number);

        Assert.DoesNotContain(result.Items, i => i.Id == 101);
        Assert.DoesNotContain(result.Items, i => i.Id == 102);
        Assert.Equal(2, result.Items.Count);

        var existingItem = result.Items.Single(i => i.Id == 100);
        Assert.Equal("Widget PRO", existingItem.ProductName);
        Assert.Equal(3, existingItem.Quantity);
        Assert.Equal(14.99m, existingItem.UnitPrice);

        var newItem = result.Items.Single(i => i.Id == 6);
        Assert.Equal("Updated", newItem.ProductName);
        Assert.Equal(10, newItem.Quantity);
        Assert.Equal(0, newItem.UnitPrice);
    }

    [Fact]
    public async Task Map_ModelToEntity_WithNestedCollection_AllNewItems()
    {
        using var ctx = EfTestHelpers.CreateSeededFullContext();
        var mapper = TestHelpers.CreateMapper();

        var model = new InvoiceModel
        {
            Id = 30,
            Number = "INV-003",
            Items = new List<LineItemModel>
            {
                new() { Id = 0, ProductName = "BrandNew", Quantity = 1, UnitPrice = 100m }
            }
        };

        var result = await mapper.BridgeTo<Invoice>(model, ctx);

        Assert.Equal(30, result.Id);
        Assert.Equal("INV-003", result.Number);
        Assert.Single(result.Items);
        Assert.Equal(0, result.Items[0].Id);
        Assert.Equal("BrandNew", result.Items[0].ProductName);
    }

    [Fact]
    public async Task Map_ModelToEntity_ExistingByPk_MapsNestedSubModel()
    {
        using var ctx = EfTestHelpers.CreateSeededFullContext();
        var mapper = TestHelpers.CreateMapper();

        var model = new CustomerModel
        {
            Id = 1,
            Name = "Acme Corp Updated"
        };

        var result = await mapper.BridgeTo<Customer>(model, ctx);

        Assert.Equal(1, result.Id);
        Assert.Equal("Acme Corp Updated", result.Name);
    }

    [Fact]
    public async Task Map_ModelToEntity_PkNotFound_CreatesNewEntity()
    {
        using var ctx = EfTestHelpers.CreateSeededFullContext();
        var mapper = TestHelpers.CreateMapper();

        var model = new CustomerModel
        {
            Id = 999,
            Name = "NewCo"
        };
        var existingCount = ctx.Customers.Count();

        var result = await mapper.BridgeTo<Customer>(model, ctx);

        Assert.Equal(999, result.Id);
        Assert.Equal("NewCo", result.Name);
        Assert.Equal(2, existingCount);
    }
}

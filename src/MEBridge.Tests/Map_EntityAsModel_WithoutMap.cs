using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

[Map<Customer>]
public class CustomerModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class EntityAsModelInput_Tests
{
    [Fact]
    public async Task Map_EntityUsedAsInput_WhenMappedAsModel()
    {
        var mapper = TestHelpers.CreateMapper();
        var model = new CustomerModel { Name = "model-as-entity" };

        using var ctx = EfTestHelpers.CreateInMemoryContext();
        var result = await mapper.BridgeTo<Customer>(model, ctx);

        Assert.Equal("model-as-entity", result.Name);
    }
}

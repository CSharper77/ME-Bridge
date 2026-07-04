using MEBridge.Attributes;

namespace MEBridge.Tests;

public class Address
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

[Bridge<Address>]
public class AddressModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
}

public class BothDirections_Tests
{
    [Fact]
    public async Task Map_EntityToModelThenModelToEntity_RoundTrips()
    {
        var mapper = TestHelpers.CreateMapper();
        using var ctx = EfTestHelpers.CreateInMemoryContext();
        var entity = new Address { Name = "round", Age = 10 };

        var model = await mapper.BridgeTo<AddressModel>(entity, null);
        var result = await mapper.BridgeTo<Address>(model, ctx);

        Assert.Equal("round", result.Name);
        Assert.Equal(10, result.Age);
    }
}

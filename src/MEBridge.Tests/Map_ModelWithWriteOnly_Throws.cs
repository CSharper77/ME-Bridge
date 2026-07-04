using MEBridge.Attributes;

namespace MEBridge.Tests;

public class WriteOnly_Entity
{
    public string Name { get; set; } = "ignored";
}

[Map<WriteOnly_Entity>]
public class WriteOnly_Model
{
    private string _name = null!;
    public string Name
    {
        set { _name = value; }
    }

    public string GetName() => _name;
}

public class WriteOnlyModel_Tests
{
    [Fact]
    public async Task Map_ModelWithWriteOnlyProperty_SetsValue()
    {
        var mapper = TestHelpers.CreateMapper();
        var entity = new WriteOnly_Entity { Name = "written" };

        var result = await mapper.BridgeTo<WriteOnly_Model>(entity, null);

        Assert.Equal("written", result.GetName());
    }
}

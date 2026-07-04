using MEBridge.Attributes;
using Microsoft.EntityFrameworkCore;

namespace MEBridge.Tests;

public class Deep5_Root
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Deep5_Level2 Level2 { get; set; } = null!;
}

public class Deep5_Level2
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Deep5_Level3 Level3 { get; set; } = null!;
}

public class Deep5_Level3
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Deep5_Level4 Level4 { get; set; } = null!;
}

public class Deep5_Level4
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public Deep5_Level5 Level5 { get; set; } = null!;
}

public class Deep5_Level5
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

[Bridge<Deep5_Root>]
public class Deep5_RootModel
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [BridgeProperty("Level2")]
    public Deep5_Level2Model Level2 { get; set; } = null!;
}

[Bridge<Deep5_Level2>]
public class Deep5_Level2Model
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [BridgeProperty("Level3")]
    public Deep5_Level3Model Level3 { get; set; } = null!;
}

[Bridge<Deep5_Level3>]
public class Deep5_Level3Model
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [BridgeProperty("Level4")]
    public Deep5_Level4Model Level4 { get; set; } = null!;
}

[Bridge<Deep5_Level4>]
public class Deep5_Level4Model
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    [BridgeProperty("Level5")]
    public Deep5_Level5Model Level5 { get; set; } = null!;
}

[Bridge<Deep5_Level5>]
public class Deep5_Level5Model
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class DeepNestedContext : DbContext
{
    public DbSet<Deep5_Root> Roots => Set<Deep5_Root>();

    public DeepNestedContext(DbContextOptions<DeepNestedContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Deep5_Root>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Level2).WithMany().HasForeignKey("Level2Id");
        });
        modelBuilder.Entity<Deep5_Level2>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Level3).WithMany().HasForeignKey("Level3Id");
        });
        modelBuilder.Entity<Deep5_Level3>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Level4).WithMany().HasForeignKey("Level4Id");
        });
        modelBuilder.Entity<Deep5_Level4>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.Level5).WithMany().HasForeignKey("Level5Id");
        });
        modelBuilder.Entity<Deep5_Level5>(e =>
        {
            e.HasKey(x => x.Id);
        });
    }
}

public class DeeplyNested5Levels_Tests
{
    private static DeepNestedContext CreateFreshContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<DeepNestedContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new DeepNestedContext(options);
    }

    [Fact]
    public async Task Map_ModelToEntity_5LevelsDeep_IncludesAutoLoadNestedEntities()
    {
        var dbName = Guid.NewGuid().ToString();

        using (var seedCtx = CreateFreshContext(dbName))
        {
            seedCtx.Roots.Add(new Deep5_Root
            {
                Id = 1,
                Name = "Root",
                Level2 = new Deep5_Level2
                {
                    Id = 2,
                    Name = "Level2",
                    Level3 = new Deep5_Level3
                    {
                        Id = 3,
                        Name = "Level3",
                        Level4 = new Deep5_Level4
                        {
                            Id = 4,
                            Name = "Level4",
                            Level5 = new Deep5_Level5 { Id = 5, Name = "Level5" }
                        }
                    }
                }
            });
            seedCtx.SaveChanges();
        }

        using var ctx = CreateFreshContext(dbName);
        var mapper = TestHelpers.CreateMapper();

        var model = new Deep5_RootModel
        {
            Id = 1,
            Name = "Root-Updated",
            Level2 = new Deep5_Level2Model
            {
                Id = 2,
                Name = "L2-Updated",
                Level3 = new Deep5_Level3Model
                {
                    Id = 3,
                    Name = "L3-Updated",
                    Level4 = new Deep5_Level4Model
                    {
                        Id = 4,
                        Name = "L4-Updated",
                        Level5 = new Deep5_Level5Model { Id = 5, Name = "L5-Updated" }
                    }
                }
            }
        };

        var result = await mapper.BridgeTo<Deep5_Root>(model, ctx);

        Assert.Equal(1, result.Id);
        Assert.Equal("Root-Updated", result.Name);
        Assert.NotNull(result.Level2);
        Assert.Equal("L2-Updated", result.Level2.Name);
        Assert.NotNull(result.Level2.Level3);
        Assert.Equal("L3-Updated", result.Level2.Level3.Name);
        Assert.NotNull(result.Level2.Level3.Level4);
        Assert.Equal("L4-Updated", result.Level2.Level3.Level4.Name);
        Assert.NotNull(result.Level2.Level3.Level4.Level5);
        Assert.Equal("L5-Updated", result.Level2.Level3.Level4.Level5.Name);
    }
}

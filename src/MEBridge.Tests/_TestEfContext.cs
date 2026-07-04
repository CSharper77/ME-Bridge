using Microsoft.EntityFrameworkCore;

namespace MEBridge.Tests;

public class TestEfContext : DbContext
{
    public DbSet<Person> People => Set<Person>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<LineItem> LineItems => Set<LineItem>();

    public TestEfContext(DbContextOptions<TestEfContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasKey(e => e.Id);
        modelBuilder.Entity<Address>().HasKey(e => e.Id);
        modelBuilder.Entity<Customer>().HasKey(e => e.Id);
        modelBuilder.Entity<Invoice>().HasKey(e => e.Id);
        modelBuilder.Entity<LineItem>().HasKey(e => e.Id);
    }
}

public static class EfTestHelpers
{
    public static TestEfContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<TestEfContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new TestEfContext(options);
    }

    public static TestEfContext CreateSeedContext<TEntity>(params TEntity[] entities) where TEntity : class
    {
        var ctx = CreateInMemoryContext();
        ctx.Set<TEntity>().AddRange(entities);
        ctx.SaveChanges();
        return ctx;
    }

    public static TestEfContext CreateSeededFullContext()
    {
        var ctx = CreateInMemoryContext();
        ctx.People.AddRange(
            new Person { Id = 1, FirstName = "Alice", LastName = "Smith", Age = 30, IsActive = true, Salary = 50000m, CreatedAt = new DateTime(2023, 1, 1) },
            new Person { Id = 2, FirstName = "Bob", LastName = "Jones", Age = 25, IsActive = false, Salary = 40000m, CreatedAt = new DateTime(2023, 2, 1) }
        );
        ctx.Addresses.AddRange(
            new Address { Id = 1, Name = "Home", Age = 5 },
            new Address { Id = 2, Name = "Work", Age = 10 }
        );
        ctx.Customers.AddRange(
            new Customer { Id = 1, Name = "Acme Corp" },
            new Customer { Id = 2, Name = "Globex Inc" }
        );
        ctx.Invoices.AddRange(
            new Invoice
            {
                Id = 10,
                Number = "INV-001",
                Items = new List<LineItem> {
                    new LineItem  { Id = 6, ProductName = "Existing item", UnitPrice = 5 }
                }
            },
            new Invoice { Id = 20, Number = "INV-002" }
        );
        ctx.LineItems.AddRange(
            new LineItem { Id = 100, ProductName = "Widget", Quantity = 2, UnitPrice = 9.99m },
            new LineItem { Id = 101, ProductName = "Gadget", Quantity = 1, UnitPrice = 29.99m },
            new LineItem { Id = 102, ProductName = "Doohickey", Quantity = 5, UnitPrice = 3.49m }
        );
        ctx.SaveChanges();
        return ctx;
    }
}

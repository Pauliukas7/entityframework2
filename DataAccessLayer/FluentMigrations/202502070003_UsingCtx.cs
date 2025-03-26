using FluentMigrator;

[Tags("Postgres")]
[Migration(202502070003)]
public class UsingCtx : Migration
{
    private readonly MyDbContext _ctx;

    public UsingCtx(MyDbContext ctx)
    {
        _ctx = ctx;
    }

    public override void Up()
    {
        Console.WriteLine("202502070003_UsingCtx");

        var categories = Enumerable.Range(0, 10).Select(cat => new Category
        {
            Id = Guid.NewGuid(),
            Name = $"Category{cat}",
            Products = Enumerable.Range(1, 10).Select(pr => new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Product{pr} of Category{cat}",
                Description = $"Product{pr} of Category{cat} Description",
                Price = 0.99m + pr
            }).ToList()
        }).ToList();

        _ctx.AddRange(categories);
        _ctx.SaveChanges();
    }

    public override void Down()
    {
        var categoryNames = Enumerable.Range(0, 10).Select(i => $"Category{i}").ToList();
        var categoriesToRemove = _ctx.Categories.Where(c => categoryNames.Contains(c.Name));

        _ctx.Categories.RemoveRange(categoriesToRemove);
        _ctx.SaveChanges();
    }
}
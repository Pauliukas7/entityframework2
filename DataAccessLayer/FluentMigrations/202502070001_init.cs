using FluentMigrator;

[Tags("Postgres")]
[Migration(202502070001)]
public class Init : Migration
{
    public override void Up()
    {
        Insert.IntoTable("Categories").Row(new
        {
            Id = Guid.NewGuid(),
            Name = "string",
            CreatedOn = DateTime.UtcNow,
            UpdatedOn = DateTime.UtcNow,
        });
    }

    public override void Down()
    {
        //Will take care in next migration
    }
}
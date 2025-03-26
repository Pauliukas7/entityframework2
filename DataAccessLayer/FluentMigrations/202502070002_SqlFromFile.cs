using FluentMigrator;

[Tags("Postgres")]
[Migration(202502070002)]
public class SqlFromFile : Migration
{
    public override void Up()
    {
        Console.WriteLine("202502070002_SqlFromFile");

        var path = @"../DataAccessLayer/FluentMigrations/Scripts/202502070002_SqlFromFile.sql";
        // Execute the script
        Execute.Script(path);
    }

    public override void Down()
    {
        throw new NotImplementedException();
    }
}
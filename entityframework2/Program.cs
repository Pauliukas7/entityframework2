
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.EntityFrameworkCore;

namespace entityframework2
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

            var connectionString = builder.Configuration.GetConnectionString("Postgres");
            builder.Services.AddDbContext<MyDbContext>(options => options.UseNpgsql(connectionString));

            builder.Services.AddFluentMigratorCore()
        .ConfigureRunner(rb => rb
        .AddPostgres()
        .WithGlobalConnectionString(connectionString)
        .WithGlobalCommandTimeout(TimeSpan.FromMinutes(15))
        .ScanIn(typeof(MyDbContext).Assembly).For.Migrations())
    .AddScoped<IMigrationRunner, MigrationRunner>()
    .AddScoped(typeof(IVersionTableMetaData), typeof(DefaultVersionTableMetaData))
    .Configure<RunnerOptions>(opt =>
    {
        opt.Tags = new[] { "Postgres" };
    });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var dbContext = services.GetRequiredService<MyDbContext>();
                    dbContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
                }
            }

            using (var scope = app.Services.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                runner.MigrateUp();
            }

            using (var scope = app.Services.CreateScope())
            {
                var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                var configuration = app.Services.GetRequiredService<IConfiguration>();
                var migrateTo = configuration["FluentMigrationsMigrations:MigrateTo"] ?? string.Empty;

                if (string.IsNullOrWhiteSpace(migrateTo))
                {
                    Console.WriteLine("No migration action taken.");
                    return;
                }

                if (migrateTo.Equals("latest", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Applying latest migrations...");
                    runner.MigrateUp();
                }
                else if (long.TryParse(migrateTo, out long targetVersion))
                {
                    Console.WriteLine($"Rolling back to migration version {targetVersion}...");
                    runner.RollbackToVersion(targetVersion);
                }
                else
                {
                    Console.WriteLine($"Invalid migration target: {migrateTo}. No action taken.");
                }
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrainingTrackerAPI.Data;
using TrainingTrackerAPI.Models;

namespace TrainingTracker.Tests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private const string TestConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=TrainingTrackerApiDb2Test;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string?>
                {
                    ["UseSqlite"] = "true",
                    ["ConnectionStrings:DefaultConnection"] = "DataSource=:memory:"
                };

                config.AddInMemoryCollection(settings);
            });

            builder.ConfigureServices(services =>
            {
                services.AddSingleton<SqliteConnection>(_ =>
                {
                    var conn = new SqliteConnection("DataSource=:memory:");
                    conn.Open();
                    return conn;
                });

                services.AddDbContext<ApplicationDbContext>((provider, options) =>
                {
                    var conn = provider.GetRequiredService<SqliteConnection>();
                    options.UseSqlite(conn);
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            return host;
        }
    }
}

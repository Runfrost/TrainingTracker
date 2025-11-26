using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TrainingTrackerAPI.Data;

namespace TrainingTracker.Tests
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        private const string TestConnectionString =
            "Server=localhost\\SQLEXPRESS;Database=TrainingTrackerApiDb2Test;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                //
                // 1. REMOVE existing DbContext (SQL Server in Program.cs)
                //
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                //
                // 2. Register SQL Server for testing
                //
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlServer(TestConnectionString);
                });
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            //
            // 3. Apply migrations & ensure DB exists
            //
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureDeleted();   // optional, for a clean run
            db.Database.EnsureCreated();

            // Or if using migrations:
            // db.Database.Migrate();

            return host;
        }
    }
}

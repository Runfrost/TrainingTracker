using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.DAL;
using TrainingTrackerAPI.Data;
using TrainingTrackerAPI.Models;


var builder = WebApplication.CreateBuilder(args);



// DbContext och connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Identity kopplad till v�r DbContext
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// NYTT
builder.Services.AddHttpClient("Backend", client =>
{
    client.BaseAddress = new Uri("https://localhost:7101");
});

builder.Services.AddScoped<ActivityAPIManager>();

builder.Services.AddRazorPages();

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();

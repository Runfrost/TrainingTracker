using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TrainingTracker.Service;
using TrainingTrackerAPI.Data;
using TrainingTrackerAPI.Models;
using TrainingTrackerAPI.Services;


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

// Identity kopplad till vår DbContext
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
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
builder.Services.AddScoped<ActivitySummaryService>();
builder.Services.AddScoped<FilterActivities>();


builder.Services.AddRazorPages(options =>
{
    // Allt under / kräver inloggning
    options.Conventions.AuthorizeFolder("/");

    // Tillåt anonym åtkomst till Identity login och register
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Login");
    options.Conventions.AllowAnonymousToAreaPage("Identity", "/Account/Register");
});


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

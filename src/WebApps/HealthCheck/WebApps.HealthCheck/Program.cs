using WebApps.HealthCheck.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddAppConfiguration();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Registers health check UI
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

builder.WebHost.UseWebRoot("wwwroot");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Add health check UI middleware
app.MapHealthChecksUI();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
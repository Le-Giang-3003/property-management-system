
using Microsoft.AspNetCore.Authentication.Cookies;
using PropertyManagementSystem.BLL;
using PropertyManagementSystem.DAL;
using PropertyManagementSystem.Web.BackgroundServices;
using PropertyManagementSystem.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("LandlordOnly", policy => policy.RequireRole("Landlord"));
    options.AddPolicy("TenantOnly", policy => policy.RequireRole("Tenant"));
    options.AddPolicy("TechnicianOnly", policy => policy.RequireRole("Technician"));
    options.AddPolicy("AdminOrLandlord", policy => policy.RequireRole("Admin", "Landlord"));
});

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer(builder.Configuration);

// Add Background Service for automatic invoice generation
builder.Services.AddHostedService<InvoiceGenerationService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Validate that authenticated users still exist in database
// This prevents access when database is reset but cookies still exist
app.UseValidateUser();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");


//Thi�n Start Route
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Maintenance}/{action=TenantIndex}");

app.Run();
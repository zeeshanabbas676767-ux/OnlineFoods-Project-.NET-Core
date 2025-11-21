using System;
using NewCoreProject.Data;
using Microsoft.EntityFrameworkCore;
using NewCoreProject.Repositories;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var builder = WebApplication.CreateBuilder(args);
// Register the Shop class with the Service Container
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
// ✅ Register IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// ✅ Add Session Support
builder.Services.AddDistributedMemoryCache(); // Required for session state
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Add MVC + Razor runtime compilation
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();

// ✅ Configure Database Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// ✅ Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Enable Session Middleware (must be before Authorization)
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

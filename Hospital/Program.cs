using Hospital.Data;
using Hospital.Models.settings;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services a los contenedores.
builder.Services.AddControllersWithViews();


//=======================DBContext=========================
builder.Services.AddDbContext<AppDBcontext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("HospitalDigital")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

// configura el HTTP requiere tuberia.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // el default HSTS valido is 30 days. tu debe esperar el cambio de este scenarios scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

using AspNetCore;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Blog.Data;
using Blog.Models;
using Blog.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options=>options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 10; config.IsDismissable = true; config.Position = NotyfPosition.BottomRight; });


//LoginPath: Bu özellik, kimlik do?rulama gerektiren bir sayfaya eri?mek istedi?inde ancak kullan?c? henüz
//oturum açmam??sa, kullan?c?n?n yönlendirilece?i giri? sayfas?n?n yolunu tan?mlar.

//Örne?in, e?er kullan?c? oturum açmadan bir admin sayfas?na eri?mek isterse, sistem onu /login yoluna
//yönlendirir. Bu, uygulaman?zda giri? yapmalar? için kullan?c?lara login sayfas?n? gösterir

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
});
var app = builder.Build();

DataSeeding();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseNotyf();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();


void DataSeeding()
{
    using(var scope = app.Services.CreateScope())
    {
        var dbInitialize = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitialize.Initialize();
    }
}
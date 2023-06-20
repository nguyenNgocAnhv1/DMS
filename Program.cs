using App.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using App;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddCors();
// thiet lapdich vu sql
var connectString = builder.Configuration.GetConnectionString("AppDb"); // chuuoi ket noi
builder.Services.AddDbContext<AppDbContext>(o =>
{
     o.UseSqlServer(connectString).LogTo(Console.WriteLine, LogLevel.None);
});
// google login
builder.Services.AddAuthentication(option =>
{
     option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
     option.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;

}).AddCookie()
.AddGoogle(GoogleDefaults.AuthenticationScheme, option =>
{
     var gconfig = builder.Configuration.GetSection("Authentication:Google");
     option.ClientId = gconfig["ClientId"];
     option.ClientSecret = gconfig["ClientSecret"];
     option.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
});
// addd session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
     o.IdleTimeout = TimeSpan.FromSeconds(1200);
     o.Cookie.HttpOnly = true;
     o.Cookie.IsEssential = true;
});
builder.Services.AddHttpContextAccessor();

var app = builder.Build();
app.UseHttpsRedirection();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
     app.UseExceptionHandler("/Home/Error");
     // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
     app.UseHsts();
}
// Enable Cors
app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseCors("AllowAnyCorsPolicy");
// khai bao su dung session
app.UseSession();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
     FileProvider = new PhysicalFileProvider(Path.Combine(
          Directory.GetCurrentDirectory(), "Upload"
     )),
     RequestPath = "/Upload"

});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseCookiePolicy(new CookiePolicyOptions()
{
     MinimumSameSitePolicy = SameSiteMode.Lax,
     // Secure =
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

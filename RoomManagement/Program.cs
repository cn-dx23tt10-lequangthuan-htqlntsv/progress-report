using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoomManagement.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình lấy Key
var jwtKey = builder.Configuration["Jwt:Key"];
if(string.IsNullOrEmpty(jwtKey) | jwtKey.Length < 32)
{
    // Cảnh báo
    throw new Exception("JWT Key must be at least 32 characters long!");
}    
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<RoomManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Authenticate/ShowLogin";
    options.LogoutPath = "/Authenticate/Logout";
    options.AccessDeniedPath = "/Authenticate/AccessDenied";
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Add services to the container.
builder.Services.AddControllersWithViews();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authenticate}/{action=ShowLogin}/{id?}");

app.Run();

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using WebDevToCSharp.Endpoints;
using WebDevToCSharp.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== JWT Configuration =====
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // Token expired tepat waktu
        };
    });

builder.Services.AddAuthorization();

// ===== MVC Configuration =====
builder.Services.AddControllersWithViews();

// ===== Dependency Injection =====
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// ===== MVC Middleware =====
app.UseRouting();

// ===== Middleware Pipeline =====
app.UseAuthentication();  // Validasi token
app.UseAuthorization();   // Cek permission

// ===== Map Endpoints =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapFileEndpoints();

app.Run();
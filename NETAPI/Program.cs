using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NETAPI.Data;
using NETAPI.Models;
using NETAPI.Repository;
using NETAPI.Services;
using NETAPI.Strategies;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Add Identity
//builder.Services.AddIdentity<User, IdentityRole<int>>()
//    .AddEntityFrameworkStores<AppDbContext>()
//    .AddDefaultTokenProviders();

var cors = "cors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: cors,
        policy =>
        {
            policy
                .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});
// JWT Configureation
var jwtKey = builder.Configuration["Jwt:Secret"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
builder.Services.AddScoped<JwtService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    return new JwtService(config);
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})

.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddScoped<AuthStrategy>();
builder.Services.AddScoped<UserAuth>();
builder.Services.AddScoped<AdminAuth>();

builder.Services.AddAuthorization();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthRepository, AuthRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(cors);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

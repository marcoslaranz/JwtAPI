// For Authetication only
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
// For Authetication only

using System.Text; // For Encoding.

using JtwRefresh.Services;
using JtwRefresh.EndPoints;

using JtwRefresh.Data;
using JtwRefresh.Repositories;

using JtwRefresh.BackgroundServices;
//using JtwRefresh.Repositories;


using Microsoft.EntityFrameworkCore;

using Npgsql.EntityFrameworkCore.PostgreSQL;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<WeatherService>();

builder.Services.AddHostedService<ExpiredTokenRemovalBackgroundService>();




// Recover from appsettings 
var issuer = builder.Configuration["JwtConfig:Issuer"] 
              ?? throw new InvalidOperationException("JwtConfig:Issuer not found!");
var audience = builder.Configuration["JwtConfig:Audience"] 
              ?? throw new InvalidOperationException("JwtConfig:Audience not found!");
var Key = builder.Configuration["JwtConfig:Key"]
               ?? throw new InvalidOperationException("JwtConfig:Key not found!");
//var tokenvalidityMins = builder.Configuration["JwtConfig:TokenValidityMins"]
//               ?? throw new InvalidOperationException("JwtConfig:TokenValidityMins not found!");
//var tokenrefreshvalidityMins = builder.Configuration["JwtConfig:RefreshTokenValidityMins"]
//                ?? throw new InvalidOperationException("JwtConfig:RefreshTokenValidityMins not found!");




builder.Services.AddDbContext<JtwRefreshDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<UserRepository>();

builder.Services.AddScoped<RefreshTokenRepository>();




// Registering Authenticaion .. (this will be used by the
// app.UseAuthentication() call below.
builder.Services.AddAuthentication(options =>
    {
		options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
		options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Key)),
			//ClockSkew = TimeSpan.Zero
			ClockSkew = TimeSpan.FromMinutes(1) // Or even 1 minute
				
        };

        // 🔍 Add logging for JWT processing
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("❌ Authentication failed: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("✅ Token validated for: " + context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });
	

// Registering Authorization
// This is related to the app.UseAuthorization below

// Basic authorization services
// builder.Services.AddAuthorization(); 

// Optional: Add policy-based authorization
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy =>
//         policy.RequireRole("Admin"));
// });

 builder.Services.AddAuthorization(); // Let's go with basic at moment

 builder.Services.AddScoped<JwtAuthenticationService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapWeatherforecast();

app.MapAccount();

app.MapUserEndPiont();

app.UseAuthentication(); // This needs a line above to register what Authenticaion method I will
                         // be using. Above you can see that I am using JwtBearerDefaults.AuthenticationScheme
						 
app.UseAuthorization();


app.Run();

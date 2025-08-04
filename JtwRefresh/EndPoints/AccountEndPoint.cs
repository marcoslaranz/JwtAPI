using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using JtwRefresh.Services;
using JtwRefresh.Models;
using JtwRefresh.DTOs;
using JtwRefresh.Repositories;
using JtwRefresh.Entities;

namespace JtwRefresh.EndPoints;

public static class AccountEndPoint
{
	public static WebApplication MapAccount(this WebApplication app)
	{
		app.MapPost("/auth/login", async (LoginRequestModel request, JwtAuthenticationService jwt) =>
		{
			var result = await jwt.Authenticate(request);
			
			Console.WriteLine($"/auth/login: request = {request.Username} {request.Password}");
			
			return result is not null ? Results.Json(result) : Results.Unauthorized();
		});
		
		app.MapPost("/auth/refresh", async (RefreshRequestModel request, JwtAuthenticationService jwt) =>
		{
			if(string.IsNullOrWhiteSpace(request.Token))
			{
				return Results.BadRequest("Invalid Token!");
			}
			
			var result = await jwt.ValidateRefreshToken(request.Token);
			
			return result is not null ? Results.Json(result): Results.Unauthorized();
		});
		return app;
	}
}

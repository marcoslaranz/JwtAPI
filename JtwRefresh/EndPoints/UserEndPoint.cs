using JtwRefresh.Services;
using JtwRefresh.Repositories;
using JtwRefresh.DTOs;
using JtwRefresh.Entities;

namespace JtwRefresh.EndPoints;

public static class UserEndPoint
{
   public static WebApplication MapUserEndPiont(this WebApplication app)
   {
		app.MapGet("/api/users/", async (UserRepository repo) => 
		{
			return await repo.GetAllAsync();
		});
		
		app.MapGet("/api/usersbyId/{Id}", async (int id, UserRepository repo) => 
		{
			return await repo.GetByIdAsync(id);
		}).WithName("GetUserById");
		
		app.MapGet("/api/usersbyName/{username}", async (string username, UserRepository repo) => 
		{
			return await repo.GetByByUserNameAsync(username);
		});
		
		app.MapPost("/api/createuser", async (UserDTO userDTO, UserRepository repo) => 
		{
			User user = await repo.CreateAsync(userDTO);
			return Results.CreatedAtRoute("GetUserById", new { id = user.Id }, user);
		});
		
		app.MapPut("/api/updateuser", async (UpdateUserDTO updateUserDTO, UserRepository repo) => 
		{
			if(updateUserDTO is null)
				return Results.BadRequest("UpdateUserDTO is required.");;
			
			User? user = await repo.UpdateUserAsync(updateUserDTO);
			if( user is null )
				 return Results.NotFound($"User with ID {updateUserDTO.Id} not found.");


			
			return Results.CreatedAtRoute("GetUserById", new { id = updateUserDTO.Id }, user);
		});
		
		return app;
   }
}

















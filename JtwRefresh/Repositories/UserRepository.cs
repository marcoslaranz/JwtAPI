using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

using JtwRefresh.Entities;
using JtwRefresh.DTOs;
using JtwRefresh.Data;
using JtwRefresh.Handlers;

namespace JtwRefresh.Repositories;

public class UserRepository
{
	private readonly JtwRefreshDbContext _dbContext;
	
	public UserRepository(JtwRefreshDbContext dbContext)
	{
		_dbContext = dbContext;
	}
	
	public async Task<IEnumerable<User?>> GetAllAsync()
	{
	    return await _dbContext.Users.ToListAsync();
	}
	 
    public async Task<User?> GetByIdAsync(int id)
	{
		return await _dbContext.Users.FindAsync(id);
	}
	
	public async Task<User?> GetByByUserNameAsync(string username)
	{
		if(username is null)
			return null;
		
		return await _dbContext.Users
						.AsNoTracking()
						.FirstOrDefaultAsync(u => u.Username == username);
	}
	
    public async Task<User> CreateAsync(UserDTO userdto)
	{
		User user = new User
		{
			Username = userdto.Username,
			PasswordHash = PasswordHashHandler.getHash(userdto.Password)
		};
		 
		_dbContext.Users.Add(user);

		await _dbContext.SaveChangesAsync();

		return user;

	}
	
	public async Task<User?> UpdateUserAsync(UpdateUserDTO updateUserDto)
	{
		var user = await _dbContext.Users.FindAsync(updateUserDto.Id);
		if( user is not null )
		{
			user.Username = updateUserDto.Username;
			user.PasswordHash = PasswordHashHandler.getHash(updateUserDto.Password);
			
			await _dbContext.SaveChangesAsync();
			
			return user;
		}else
		{
			return null;
		}
	}
}
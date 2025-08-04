using System.Text; // For Encoding.

using System.IdentityModel.Tokens.Jwt;

using Microsoft.IdentityModel.Tokens;

using System.Security.Claims;


using JtwRefresh.Models;
using JtwRefresh.Repositories;
using JtwRefresh.Handlers;
using JtwRefresh.Entities;


namespace JtwRefresh.Services;

public class JwtAuthenticationService
{
	private readonly IConfiguration _configuration;
	private readonly UserRepository _userRepository;
	private readonly RefreshTokenRepository _refreshTokenRepository;
	
	public JwtAuthenticationService(IConfiguration configuration, UserRepository userRepository,
	RefreshTokenRepository refreshTokenRepository)
	{
		_configuration = configuration;
	    _userRepository = userRepository;
		_refreshTokenRepository = refreshTokenRepository;
	}
	
	public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
	{
		if(string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
		{
			return null;
		}
		
		var user = await _userRepository.GetByByUserNameAsync(request.Username);
		if(user is null || !PasswordHashHandler.VerifyPassword(request.Password, user.PasswordHash!))
		{
			return null;
		}
		
		return await GenerateJwtToken(user);
	}
	
	
	
	
	public async Task<LoginResponseModel?> GenerateJwtToken(User user)
	{
		// Recover from appsettings 
		var issuer = _configuration["JwtConfig:Issuer"] 
					  ?? throw new InvalidOperationException("JwtConfig:Issuer not found!");
		var audience = _configuration["JwtConfig:Audience"] 
					  ?? throw new InvalidOperationException("JwtConfig:Audience not found!");
		var Key = _configuration["JwtConfig:Key"]
					   ?? throw new InvalidOperationException("JwtConfig:Key not found!");
					   
		var tokenvalidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
		if (tokenvalidityMins == 0)
		{
			throw new InvalidOperationException("JwtConfig:TokenValidityMins not found!");
		}
					   
		var tokenrefreshvalidityMins = _configuration["JwtConfig:RefreshTokenValidityMins"]
						?? throw new InvalidOperationException("JwtConfig:RefreshTokenValidityMins not found!");
						
		var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenvalidityMins);
		var key = Encoding.UTF8.GetBytes(Key);
		
		var token = new JwtSecurityToken(
				issuer,
				audience,
				[
					//new Claim(JwtRegisteredClaimNames.Name, user.Username!)
					 new Claim(ClaimTypes.Name, user.Username!),
                     new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())  // Also recommended
				],
				expires: tokenExpiryTimeStamp,
				signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
											  SecurityAlgorithms.HmacSha512Signature)
				);
				
		var accessToken = new JwtSecurityTokenHandler().WriteToken(token);	
		
		return new LoginResponseModel
		{
			Username = user.Username,
			AccessToken = accessToken,
			ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
			RefreshToken = await GenerateRefreshToken(user.Id)
		};
	}
	
	
	private async Task<string> GenerateRefreshToken(int userId)
	{
		var refreshTokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
		if (refreshTokenValidityMins == 0)
		{
			throw new InvalidOperationException("JwtConfig:TokenValidityMins not found!");
		}
		
		var refreshToken = new RefreshToken
		{
			Token = Guid.NewGuid().ToString(),
			Expiry = DateTime.UtcNow.AddMinutes(refreshTokenValidityMins),
			UserId = userId
		};
		
		await _refreshTokenRepository.CreateAsync(refreshToken);
		
		return refreshToken.Token;
	}
	
	
	
	
	
	
	
	
	public async Task<LoginResponseModel?> ValidateRefreshToken(string token)
	{
		var refreshToken = await _refreshTokenRepository.Get(token);
		if(refreshToken is null || refreshToken.Expiry < DateTime.UtcNow)
		{
			return null;
		}
		
		await _refreshTokenRepository.Delete(refreshToken);
		
		var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
		if(user is null) 
		{
			return null;
		}
		return await GenerateJwtToken(user);
	}
	
	
	
	
	
}
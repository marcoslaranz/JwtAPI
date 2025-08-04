using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JtwRefresh.Entities;
namespace JtwRefresh.Data;


public class JtwRefreshDbContext(DbContextOptions<JtwRefreshDbContext> options):DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
	
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		//modelBuilder.Entity<RefreshToken>()
		//	          .HasKey(rt => rt.Token);
					  
		modelBuilder.Entity<User>().HasData(
		   new {
			   Id = 1,
			   Username = "admin",
			   PasswordHash = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
		   }
		);
    }
}

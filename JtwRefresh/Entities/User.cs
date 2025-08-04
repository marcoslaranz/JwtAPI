namespace JtwRefresh.Entities;


public class User
{
	public int Id { get; set; }
	public required string Username { get; set; }
	public required string PasswordHash { get; set; }
}


/*
	This is optional to create a table in the database:
	

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("User")]
public class User
{
	[Key]
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Column("id")]
	public int Id { get; set; }
	
	[Column("user_name")]
	public required string Username { get; set; }
	
	[Column("password_hash")]
	public required string PasswordHash { get; set; }
	
}
*/
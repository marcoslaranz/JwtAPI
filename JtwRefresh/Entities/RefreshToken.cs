namespace JtwRefresh.Entities;

public class RefreshToken
{
	public int Id { get; set; }
	public int UserId { get; set; }
	public string? Token { get; set; }
	public DateTime	Expiry  { get; set; }
}


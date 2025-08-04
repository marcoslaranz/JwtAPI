using JtwRefresh.Services;

namespace JtwRefresh.Services;

public class WeatherService
{
	private readonly string[] summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	public IEnumerable<WeatherForecast> GetWeatherforecast()
	{
		return Enumerable.Range(1, 5).Select(index =>
			new WeatherForecast
			(
				DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
				Random.Shared.Next(-20, 55),
				summaries[Random.Shared.Next(summaries.Length)]
			))
			.ToArray();
	}
}

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
using JtwRefresh.Services;

namespace JtwRefresh.EndPoints;

public static class WeatherEndPoint
{
	public static WebApplication MapWeatherforecast(this WebApplication app)
	{
		app.MapGet("/weatherforecast", (WeatherService service) =>
		{
			return service.GetWeatherforecast();
		})
		.WithName("GetWeatherForecast")
		.RequireAuthorization();
		
		return app;
	}
}
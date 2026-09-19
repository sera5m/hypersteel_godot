namespace Hypersteel.Weather;

public static class WeatherStatusPaint
{
	public static readonly string[] Empty = System.Array.Empty<string>();

	public static string[] TagsFor(WeatherRowId row) => row switch
	{
		WeatherRowId.Rain or WeatherRowId.Downpour => new[] { "Wet" },
		WeatherRowId.Snow => new[] { "Cold" },
		WeatherRowId.Heat => new[] { "Hot" },
		WeatherRowId.Steam => new[] { "Wet", "Hot" },
		WeatherRowId.Smoke => new[] { "Sanded" },
		WeatherRowId.Smog => new[] { "Toxic" },
		WeatherRowId.AcidRain => new[] { "Toxic" },
		WeatherRowId.Spores => new[] { "Toxic" },
		WeatherRowId.Dust => new[] { "Sanded" },
		WeatherRowId.Aurora => new[] { "Zapped" },
		WeatherRowId.ExplosiveGas => new[] { "ExplosiveCoated" },
		WeatherRowId.EclipseAthenaMars => new[] { "Zapped" },
		_ => Empty,
	};
}

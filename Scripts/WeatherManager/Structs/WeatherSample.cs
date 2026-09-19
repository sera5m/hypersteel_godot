namespace Hypersteel.Weather;

public struct WeatherSample
{
	public WeatherRowId Row;
	public WeatherRarity Rarity;
	public PlanetId Planet;
	public AtmosphereKind Atmosphere;
	public WindSample Wind;
	public float Intensity;
	public float AirTempC;
	public bool Indoor;
	public OverlayKind Overlay;
}

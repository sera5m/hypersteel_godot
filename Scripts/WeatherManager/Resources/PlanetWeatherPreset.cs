using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class PlanetWeatherPreset : Resource
{
	[Export] public PlanetId planet = PlanetId.Terra;
	[Export] public AtmosphereKind atmosphere = AtmosphereKind.Air;
	[Export] public WeatherRowDef[] normal = System.Array.Empty<WeatherRowDef>();
	[Export] public WeatherRowDef[] abnormal = System.Array.Empty<WeatherRowDef>();
	[Export] public WeatherRowDef[] rare = System.Array.Empty<WeatherRowDef>();
}

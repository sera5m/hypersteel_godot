using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class LevelWeatherDef : Resource
{
	[Export] public PlanetWeatherPreset planet;
	[Export] public WeatherRowDef row;
	[Export] public WindPropagationMap windMap;
	[Export] public Vector3 prevailingWind = new(1f, 0f, 0f);
	[Export] public WindTier prevailingTier = WindTier.Wind;
	[Export] public bool allowLiveRaycast;
	[Export] public string sequenceName = "";
}

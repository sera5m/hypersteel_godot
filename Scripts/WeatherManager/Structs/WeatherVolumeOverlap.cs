using Godot;

namespace Hypersteel.Weather;

public struct WeatherVolumeOverlap
{
	public WeatherRowId Row;
	public float Intensity;
	public Aabb Bounds;
}

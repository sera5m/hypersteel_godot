using Godot;
using System.Collections.Generic;

namespace Hypersteel.Weather;

public partial class WeatherManager : Node
{
	public static WeatherManager Instance { get; private set; }

	public LevelWeatherDef Def { get; private set; }
	public WindPropagationMap WindMap { get; private set; }
	public WeatherRowDef Row { get; private set; }
	public PlanetWeatherPreset Planet { get; private set; }

	readonly List<WeatherVolumeOverlap> _volumes = new();
	WindPropagationBaker _baker;

	[Signal] public delegate void WeatherChangedEventHandler();

	public override void _EnterTree() => Instance = this;
	public override void _ExitTree() { if (Instance == this) Instance = null; }

	public void Apply(LevelWeatherDef def)
	{
		Def = def;
		Planet = def?.planet;
		Row = def?.row;
		WindMap = def?.windMap;
		EmitSignal(SignalName.WeatherChanged);
	}

	public void SetRow(WeatherRowDef row)
	{
		Row = row;
		EmitSignal(SignalName.WeatherChanged);
	}

	public void AddVolume(WeatherVolumeOverlap volume) => _volumes.Add(volume);
	public void ClearVolumes() => _volumes.Clear();

	public WindSample SampleWind(Vector3 world)
		=> WindMap != null ? WindMap.Sample(world) : WindSample.Still;

	public WeatherSample Sample(Vector3 world, bool indoor = false)
	{
		var row = Row?.id ?? WeatherRowId.Clear;
		var intensity = Row?.intensity ?? 0f;
		var overlay = Row?.overlay ?? OverlayKind.None;
		var temp = Row?.airTempC ?? 18f;
		for (int i = 0; i < _volumes.Count; i++)
		{
			if (_volumes[i].Bounds.HasPoint(world))
			{
				row = _volumes[i].Row;
				intensity = Mathf.Max(intensity, _volumes[i].Intensity);
			}
		}
		var atmo = Planet?.atmosphere ?? AtmosphereKind.Air;
		if (atmo == AtmosphereKind.Vacuum) indoor = true;
		return new WeatherSample
		{
			Row = row,
			Rarity = Row?.rarity ?? WeatherRarity.Normal,
			Planet = Planet?.planet ?? PlanetId.Terra,
			Atmosphere = atmo,
			Wind = SampleWind(world),
			Intensity = indoor ? intensity * 0.25f : intensity,
			AirTempC = temp,
			Indoor = indoor,
			Overlay = overlay,
		};
	}

	public WindPropagationMap BakeWind(World3D world, Vector3 origin, Vector3I dim, Vector3 cellSize, Vector3 prevailing, WindTier tier)
	{
		_baker ??= new WindPropagationBaker();
		WindMap = _baker.Bake(world, origin, dim, cellSize, prevailing, tier);
		return WindMap;
	}

	public string[] StatusTags()
	{
		if (Row != null && Row.statusTags is { Length: > 0 }) return Row.statusTags;
		return WeatherStatusPaint.TagsFor(Row?.id ?? WeatherRowId.Clear);
	}
}

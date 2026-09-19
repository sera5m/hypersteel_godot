using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class LevelWeatherDirector : Node
{
	[Export] public LevelWeatherDef def;
	[Export] public bool bakeOnReadyIfMissing;
	[Export] public Vector3 bakeOrigin;
	[Export] public Vector3I bakeDim = new(32, 8, 32);
	[Export] public Vector3 bakeCell = new(4f, 4f, 4f);

	public override void _Ready()
	{
		if (WeatherManager.Instance == null) { GD.PushWarning("WeatherManager autoload missing."); return; }
		if (def == null) return;
		if (def.windMap == null && bakeOnReadyIfMissing)
		{
			var world = GetViewport()?.World3D;
			if (world != null)
				def.windMap = WeatherManager.Instance.BakeWind(world, bakeOrigin, bakeDim, bakeCell, def.prevailingWind, def.prevailingTier);
		}
		WeatherManager.Instance.Apply(def);
	}

	public void PlayRare(WeatherRowDef rare) => WeatherManager.Instance?.SetRow(rare);
	public void ClearRare()
	{
		if (def?.row != null) WeatherManager.Instance?.SetRow(def.row);
	}
}

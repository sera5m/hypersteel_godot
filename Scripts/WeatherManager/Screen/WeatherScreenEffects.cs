using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class WeatherScreenEffects : Node
{
	[Export] public OverlayKind current = OverlayKind.None;
	[Export(PropertyHint.Range, "0,1")] public float strength;
	public void Apply(WeatherSample sample)
	{
		current = sample.Overlay;
		strength = sample.Indoor ? sample.Intensity * 0.25f : sample.Intensity;
	}
}

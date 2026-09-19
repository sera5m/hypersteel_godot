using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class WeatherAudioBed : Node
{
	[Export] public string bedName = "";
	[Export] public float reverbSend;
	[Export] public bool suitOnly;
	public void Apply(WeatherSample sample)
	{
		suitOnly = sample.Atmosphere == AtmosphereKind.Vacuum;
		if (suitOnly) { bedName = ""; reverbSend = 0f; return; }
		bedName = sample.Indoor ? "interior" : sample.Row.ToString().ToLowerInvariant();
		reverbSend = sample.Indoor ? 0.35f : 0.1f;
	}
}

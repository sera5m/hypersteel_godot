using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class WeatherRowDef : Resource
{
	[Export] public WeatherRowId id = WeatherRowId.Clear;
	[Export] public WeatherRarity rarity = WeatherRarity.Normal;
	[Export] public OverlayKind overlay = OverlayKind.None;
	[Export(PropertyHint.Range, "0,1")] public float intensity = 0.5f;
	[Export] public float airTempC = 18f;
	[Export] public string[] statusTags = System.Array.Empty<string>();
	[Export] public string bedName = "";
	[Export] public float reverbSend;
}

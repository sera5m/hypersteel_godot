using Godot;

namespace Hypersteel.Weapons;

/// <summary>Apex-style optic. World pickup. Mounts on Gun.</summary>
[GlobalClass]
public partial class SightDef : Resource
{
	[Export] public string Id = "1x";
	[Export] public float ZoomMin = 1f;
	[Export] public float ZoomMax = 1f;
	[Export] public bool Variable;
	[Export] public bool SniperOnly;
	[Export] public bool CloseBias;

	public float MidZoom => (ZoomMin + ZoomMax) * 0.5f;
	public bool IsClose => ZoomMax <= 1.5f || CloseBias;
	public bool IsFar => ZoomMin >= 3f;

	public static SightDef Iron() => new() { Id = "iron", ZoomMin = 1f, ZoomMax = 1f, CloseBias = true };
	public static SightDef Hcog1x() => new() { Id = "1x", ZoomMin = 1f, ZoomMax = 1f, CloseBias = true };
	public static SightDef Holo12() => new() { Id = "1-2x", ZoomMin = 1f, ZoomMax = 2f, Variable = true };
	public static SightDef Bruiser2x() => new() { Id = "2x", ZoomMin = 2f, ZoomMax = 2f };
	public static SightDef Ranger3x() => new() { Id = "3x", ZoomMin = 3f, ZoomMax = 3f };
	public static SightDef Var24() => new() { Id = "2-4x", ZoomMin = 2f, ZoomMax = 4f, Variable = true };
	public static SightDef Sniper48() => new() { Id = "4-8x", ZoomMin = 4f, ZoomMax = 8f, Variable = true, SniperOnly = true };

	public static SightDef FromId(string id) => (id ?? "").ToLowerInvariant() switch
	{
		"1-2x" or "holo12" => Holo12(),
		"2x" => Bruiser2x(),
		"3x" => Ranger3x(),
		"2-4x" or "24" => Var24(),
		"4-8x" or "6x" or "sniper" => Sniper48(),
		"iron" => Iron(),
		_ => Hcog1x(),
	};
}

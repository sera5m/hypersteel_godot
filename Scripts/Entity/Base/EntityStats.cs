using Godot;

namespace Hypersteel.Entity;

/// <summary>
/// MP operator sheet. Every ActorEntity has one. AI does not get a second table.
/// Meat baseline in MP is already +cyber; raw meat is ~20% under.
/// </summary>
[GlobalClass]
public partial class EntityStats : Resource
{
	[Export] public float Mass = 80f;
	[Export] public float Agility = 1f;
	[Export] public float Speed = 1f;
	[Export] public float Stamina = 1f;
	[Export] public float Strength = 1f;
	[Export] public float UseTime = 1f;
	[Export] public float AbilityCd = 1f;
	[Export] public float Recoil = 1f;
	[Export] public float Thermal = 1f;
	[Export] public float HealRate = 1f;
	[Export] public float TurnRateDeg = 320f;

	[Export] public float Reaction = 0.28f;
	[Export] public float LayerReaction = 0.08f;
	[Export] public float VisionConeDeg = 110f;
	[Export] public float AimConeDeg = 55f;
	[Export] public float AimSlackDeg = 30f;
	[Export] public float TheyFaceMeDeg = 45f;
	[Export] public float PreRotate = 0.2f;
	[Export] public float FearResist = 0.2f;
	[Export] public int Rank;
	[Export] public bool SmartPriority;

	public float HandSpeed => Mathf.Max(0.15f, Agility / Mathf.Max(0.15f, UseTime));

	public float RecoilScale => Recoil / Mathf.Max(0.15f, 0.5f * Strength + 0.5f * Agility);

	public float LiveTurnRate(float gunSlowdown01) =>
		TurnRateDeg * (1f - Mathf.Clamp(gunSlowdown01, 0f, 0.5f));
}

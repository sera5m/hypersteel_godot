using Godot;

namespace Hypersteel.Damage;

public enum ProjectileClass
{
	Ballistic = 0,
	Hitscan,
	Grenade,
	Plasma,
	Missile,
	Lightning,
	Flame,
}

public enum ProjectileDeath
{
	Delete = 0,
	Shrapnel,
	Detonate,
}

[GlobalClass]
public partial class ProjectileDef : Resource
{
	[Export] public StringName defId = "ballistic";
	[Export] public ProjectileClass kind;
	[Export] public ProjectileDeath death = ProjectileDeath.Delete;

	[ExportGroup("Packet template")]
	[Export] public float kinetic = 40f;
	[Export] public int ap = 3;
	[Export] public KineticFlags flags = KineticFlags.Piercing;
	[Export] public bool wakeCavitation;
	[Export] public bool followRealVel;
	[Export] public float crossSectionCm2 = 0.7f;

	[ExportGroup("Flight")]
	[Export] public float speed = 80f;
	[Export] public float gravity = 0f;
	[Export] public float drag = 0f;
	[Export] public float lifetime = 4f;
	[Export] public bool debugTrace;

	[ExportGroup("Impact spawn")]
	[Export] public bool stickOnHit;
	[Export] public PackedScene spawnOnHit;
	[Export] public PackedScene spawnOnWorld;

	[ExportGroup("Shrapnel")]
	[Export] public int shrapnelCount;
	[Export] public bool shrapnelOnExit;
}

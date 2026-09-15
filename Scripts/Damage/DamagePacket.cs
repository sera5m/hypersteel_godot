using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

public enum DamageSource
{
	Projectile = 0,
	Fist,
	Blade,
	Beam,
	Splash,
	Contact,
	Environment,
}

[System.Flags]
public enum KineticFlags
{
	None = 0,
	Piercing = 1 << 0,
	Blunt = 1 << 1,
	Fragmenting = 1 << 2,
	Hypervelocity = 1 << 3,
	Cutting = 1 << 4,
	Incendiary = 1 << 5,
}

public enum ApOutcome
{
	Null = 0,
	Bounce,
	Damage,
	Overpen,
	Cavitation,
}

public struct DamageChannel
{
	public DamageKind Kind;
	public float Amount;

	public DamageChannel(DamageKind kind, float amount)
	{
		Kind = kind;
		Amount = amount;
	}
}

public struct ShrapnelRecipe
{
	public int Fragments;
	public bool InheritDot;
	public bool Piercing;
	public bool SpawnWorldIfExit;
}

/// <summary>One impact. Projectile code fills this; Resolve reads it.</summary>
public struct DamagePacket
{
	public StringName Name;
	public DamageSource Source;
	public int Ap;
	public float Kinetic;
	public KineticFlags KineticFlags;
	public DamageChannel[] Channels;
	public ShrapnelRecipe Shrapnel;
	public bool HasShrapnel;
	public Vector3 Point;
	public Vector3 Normal;
	public StringName Bone;
	public BodySegment Segment;
	public Node3D Instigator;

	public static DamagePacket KineticHit(float amount, int ap, DamageSource source = DamageSource.Projectile)
	{
		return new DamagePacket
		{
			Name = "hit",
			Source = source,
			Ap = ap,
			Kinetic = amount,
			KineticFlags = KineticFlags.Piercing,
			Segment = BodySegment.Torso,
		};
	}

	public Hit ToLegacyHit()
	{
		DamageKind kind = DamageKind.Kinetic;
		if (Channels != null)
		{
			for (int i = 0; i < Channels.Length; i++)
				kind |= Channels[i].Kind;
		}
		return new Hit(Kinetic, kind, Segment, Point, Normal, Instigator, Bone);
	}
}

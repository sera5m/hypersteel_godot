namespace Hypersteel.Health;

[System.Flags]
public enum DamageKind
{
	None = 0,
	Kinetic = 1 << 0,
	Plasma = 1 << 1,
	Microwave = 1 << 2,
	Explosive = 1 << 3,
	True = 1 << 4,
}

public enum BodySegment
{
	Torso = 0,
	Head,
	ArmL,
	ArmR,
	HandL,
	HandR,
	LegL,
	LegR,
	Generic,
}

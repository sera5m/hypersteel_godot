using Godot;

namespace Hypersteel.Damage;

[System.Flags]
public enum StatusTag
{
	None = 0,
	Burning = 1 << 0,
	Sanded = 1 << 1,
	Hot = 1 << 2,
	Cold = 1 << 3,
	Wet = 1 << 4,
	Oiled = 1 << 5,
	ExplosiveCoated = 1 << 6,
	Bleeding = 1 << 7,
	LeakingFuel = 1 << 8,
	LeakingCoolant = 1 << 9,
	LeakingPower = 1 << 10,
	Stunned = 1 << 11,
	Paralyzed = 1 << 12,
	Zapped = 1 << 13,
	Blinded = 1 << 14,
	Deafened = 1 << 15,
	Ragdolled = 1 << 16,
	Liquefied = 1 << 17,
	Stimmed = 1 << 18,
	Tranqued = 1 << 19,
	Amped = 1 << 20,
	Enraged = 1 << 21,
	Charged = 1 << 22,
	Shattered = 1 << 23,
	Sliced = 1 << 24,
}

[GlobalClass]
public partial class StatusRules : Resource
{
	[Export] public float ignitionTemp = 300f;
	[Export] public float healPerSecond = 0f;
	[Export] public float toxicHealDivisor = 10000f;
	[Export] public bool flesh = true;
	[Export] public bool machine;
}

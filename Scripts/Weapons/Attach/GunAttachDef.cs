using Godot;

namespace Hypersteel.Weapons.Attach;

public enum GunAttachSlot
{
	Optic = 0,
	Muzzle,
	Barrel,
	Stock,
	Mag,
	Under,
	Rail,
	Hopup,
}

[System.Flags]
public enum GunAmmoFamily
{
	None = 0,
	Light = 1,
	Rifle = 2,
	Fifty = 4,
	Shells = 8,
	RailS = 16,
	RailL = 32,
	Plasma = 64,
	Micro = 128,
	Rocket = 256,
	Twenty = 512,
	Forty = 1024,
	AnyKinetic = Light | Rifle | Fifty,
}

[GlobalClass]
public partial class GunAttachDef : Resource
{
	[Export] public string Id = "";
	[Export] public GunAttachSlot Slot;
	[Export] public GunAmmoFamily AmmoMask = GunAmmoFamily.Rifle;
	[Export] public float Recoil = 1f;
	[Export] public float Spread = 1f;
	[Export] public float Ads = 1f;
	[Export] public float HandlingAdd;
	[Export] public float Reload = 1f;
	[Export] public float MagMul = 1f;
	[Export] public float Vel = 1f;
	[Export] public float Heat = 1f;
	[Export] public float Sound = 1f;
	[Export] public int ApAdd;
	[Export] public string ExtraStatus = "";

	public bool Fits(GunAmmoFamily fam) => (AmmoMask & fam) != 0;
}

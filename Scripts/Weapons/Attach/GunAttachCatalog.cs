namespace Hypersteel.Weapons.Attach;

public static class GunAttachCatalog
{
	public static GunAttachDef MagGrip() => new()
	{
		Id = "mag_grip", Slot = GunAttachSlot.Mag, AmmoMask = GunAmmoFamily.AnyKinetic | GunAmmoFamily.Shells | GunAmmoFamily.Plasma,
		MagMul = 0.7f, Reload = 0.85f, HandlingAdd = -0.04f
	};

	public static GunAttachDef MagStd() => new()
	{
		Id = "mag_std", Slot = GunAttachSlot.Mag, AmmoMask = ~GunAmmoFamily.None, MagMul = 1f
	};

	public static GunAttachDef MagBox() => new()
	{
		Id = "mag_box", Slot = GunAttachSlot.Mag, AmmoMask = GunAmmoFamily.Rifle | GunAmmoFamily.Light | GunAmmoFamily.Plasma,
		MagMul = 1.8f, Reload = 1.35f, HandlingAdd = 0.08f
	};

	public static GunAttachDef StockNone() => new()
	{
		Id = "stock_none", Slot = GunAttachSlot.Stock, AmmoMask = ~GunAmmoFamily.None,
		HandlingAdd = -0.06f, Spread = 1.2f, Ads = 1.15f
	};

	public static GunAttachDef StockRecoil() => new()
	{
		Id = "stock_recoil", Slot = GunAttachSlot.Stock, AmmoMask = GunAmmoFamily.Rifle | GunAmmoFamily.Fifty,
		Recoil = 0.82f, HandlingAdd = 0.03f
	};

	public static GunAttachDef StockPrecision() => new()
	{
		Id = "stock_precision", Slot = GunAttachSlot.Stock, AmmoMask = GunAmmoFamily.Rifle | GunAmmoFamily.Fifty,
		Ads = 0.88f, HandlingAdd = 0.05f, Spread = 0.9f
	};

	public static GunAttachDef StockBrace() => new()
	{
		Id = "stock_brace", Slot = GunAttachSlot.Stock, AmmoMask = GunAmmoFamily.Light,
		Spread = 0.55f, HandlingAdd = 0.04f, Ads = 0.9f, Reload = 1.1f
	};

	public static GunAttachDef RifleBrake() => new()
	{
		Id = "mz_brake", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Rifle,
		Recoil = 0.72f, Sound = 1.25f
	};

	public static GunAttachDef RifleCan() => new()
	{
		Id = "mz_can", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Rifle | GunAmmoFamily.Light,
		Sound = 0.45f, Vel = 0.92f, Heat = 1.15f
	};

	public static GunAttachDef RifleWash() => new()
	{
		Id = "mz_wash", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Rifle,
		Heat = 1.2f, ExtraStatus = "Heat"
	};

	public static GunAttachDef LightComp() => new()
	{
		Id = "mz_comp", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Light, Recoil = 0.8f
	};

	public static GunAttachDef ShellSpreader() => new()
	{
		Id = "mz_spread", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Shells, Spread = 1.55f, Recoil = 0.95f
	};

	public static GunAttachDef ShellChoke() => new()
	{
		Id = "mz_choke", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Shells, Spread = 0.7f
	};

	public static GunAttachDef RailStraight() => new()
	{
		Id = "mz_field", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.RailS | GunAmmoFamily.RailL,
		Vel = 1.08f, Heat = 1.1f
	};

	public static GunAttachDef PlasmaTight() => new()
	{
		Id = "mz_tight", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Plasma, Vel = 1.15f, Spread = 0.8f
	};

	public static GunAttachDef PlasmaWide() => new()
	{
		Id = "mz_wide", Slot = GunAttachSlot.Muzzle, AmmoMask = GunAmmoFamily.Plasma, Spread = 1.4f, Vel = 0.85f
	};

	public static GunAttachDef MicroGuide() => new()
	{
		Id = "hp_guide", Slot = GunAttachSlot.Hopup, AmmoMask = GunAmmoFamily.Micro, ExtraStatus = "smart"
	};

	public static GunAttachDef HopShock() => new()
	{
		Id = "hp_shock", Slot = GunAttachSlot.Hopup, AmmoMask = GunAmmoFamily.AnyKinetic | GunAmmoFamily.Plasma, ExtraStatus = "Shock"
	};

	public static GunAttachDef HopAp() => new()
	{
		Id = "hp_ap", Slot = GunAttachSlot.Hopup, AmmoMask = GunAmmoFamily.AnyKinetic, ApAdd = 1, Vel = 0.94f
	};

	public static GunAttachDef UnderGrip() => new()
	{
		Id = "un_grip", Slot = GunAttachSlot.Under, AmmoMask = ~GunAmmoFamily.None, HandlingAdd = -0.05f, Ads = 1.04f
	};

	public static GunAttachDef RailLaser() => new()
	{
		Id = "rl_laser", Slot = GunAttachSlot.Rail, AmmoMask = ~GunAmmoFamily.None, Spread = 0.88f
	};
}

using Godot;
using System.Collections.Generic;
using Hypersteel.Weapons.Attach;

namespace Hypersteel.Weapons;

public partial class Gun : Node3D
{
	[Export] public GunUseHint Use;
	[Export] public bool AcceptsSniperGlass = true;
	[Export] public bool HasCanted = true;
	[Export] public GunAmmoFamily Ammo = GunAmmoFamily.Rifle;
	[Export] public float BaseHandling = 0.30f;

	public SightDef Mounted { get; private set; }
	public bool CantedLive { get; private set; }
	public readonly List<SightDef> Bag = new();
	public readonly GunKit Kit = new();

	public SightDef Live => CantedLive || Mounted == null ? SightDef.Hcog1x() : Mounted;

	public bool MountAttach(GunAttachDef def) => Kit.TryMount(def, Ammo);

	public float TurnSlowdown(float strength) => GunKit.TurnSlowdown(Kit.Handling == 0f ? BaseHandling : Kit.Handling, strength);

	public float ReloadFor(float baseReload, float agility, float useTime) =>
		GunKit.ReloadSeconds(baseReload, Kit.Reload, agility, useTime);

	public bool Mount(SightDef sight)
	{
		if (sight == null) return false;
		if (sight.SniperOnly && !AcceptsSniperGlass) return false;
		if (Mounted != null && Mounted.Id != "iron") Bag.Add(Mounted);
		Mounted = sight;
		CantedLive = false;
		return true;
	}

	public SightDef Strip()
	{
		var old = Mounted;
		Mounted = SightDef.Iron();
		CantedLive = false;
		return old != null && old.Id != "iron" ? old : null;
	}

	public void SetCanted(bool on)
	{
		if (!HasCanted) { CantedLive = false; return; }
		CantedLive = on && Mounted != null && !Mounted.IsClose;
	}

	public bool EquipBestFor(float dist, float close, float far)
	{
		if (dist <= close)
		{
			if (Mounted != null && !Mounted.IsClose && HasCanted) { SetCanted(true); return true; }
			var oneX = TakeFromBag(s => s.IsClose);
			if (oneX != null) return Mount(oneX);
			if (Mounted != null && Mounted.IsFar)
			{
				var stripped = Strip();
				if (stripped != null) Bag.Add(stripped);
				return true;
			}
			return false;
		}
		if (dist >= far)
		{
			SetCanted(false);
			if (Mounted != null && Mounted.IsFar) return false;
			var glass = TakeFromBag(s => s.IsFar || s.ZoomMax >= 3f);
			return glass != null && Mount(glass);
		}
		SetCanted(false);
		return false;
	}

	SightDef TakeFromBag(System.Func<SightDef, bool> pred)
	{
		for (int i = 0; i < Bag.Count; i++)
		{
			if (!pred(Bag[i])) continue;
			var s = Bag[i];
			Bag.RemoveAt(i);
			return s;
		}
		return null;
	}
}

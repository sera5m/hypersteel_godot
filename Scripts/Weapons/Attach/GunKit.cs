using Godot;
using System.Collections.Generic;

namespace Hypersteel.Weapons.Attach;

/// <summary>One attach per slot. Handling cap 50%. Reload uses Agility.</summary>
public sealed class GunKit
{
	public readonly Dictionary<GunAttachSlot, GunAttachDef> Slots = new();

	public float Recoil = 1f;
	public float Spread = 1f;
	public float Ads = 1f;
	public float Handling = 0.30f;
	public float Reload = 1f;
	public float MagMul = 1f;
	public float Vel = 1f;
	public float Heat = 1f;
	public float Sound = 1f;
	public int ApAdd;

	public bool TryMount(GunAttachDef def, GunAmmoFamily fam)
	{
		if (def == null || !def.Fits(fam)) return false;
		Slots[def.Slot] = def;
		Recompute();
		return true;
	}

	public void Clear(GunAttachSlot slot)
	{
		Slots.Remove(slot);
		Recompute();
	}

	public void Recompute(float baseHandling = 0.30f)
	{
		Recoil = Spread = Ads = Reload = MagMul = Vel = Heat = Sound = 1f;
		Handling = baseHandling;
		ApAdd = 0;
		foreach (var a in Slots.Values)
		{
			Recoil *= a.Recoil;
			Spread *= a.Spread;
			Ads *= a.Ads;
			Reload *= a.Reload;
			MagMul *= a.MagMul;
			Vel *= a.Vel;
			Heat *= a.Heat;
			Sound *= a.Sound;
			Handling += a.HandlingAdd;
			ApAdd += a.ApAdd;
		}
		Handling = Mathf.Clamp(Handling, 0f, 0.5f);
	}

	/// <summary>Turn-rate slowdown 0-50%. Rifle H=0.30 at Strength 1 = 30%.</summary>
	public static float TurnSlowdown(float handling, float strength) =>
		Mathf.Clamp(handling / Mathf.Max(0.05f, strength), 0f, 0.5f);

	/// <summary>Reload seconds. Hand speed = Agility / UseTime.</summary>
	public static float ReloadSeconds(float baseReload, float kitReload, float agility, float useTime)
	{
		var hands = Mathf.Max(0.15f, agility / Mathf.Max(0.15f, useTime));
		return baseReload * kitReload / hands;
	}
}

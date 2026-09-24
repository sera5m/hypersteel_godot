using Godot;
using Hypersteel.Entity;
using System.Collections.Generic;

namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcRangeBand
{
	Point = 0,
	Close,
	Mid,
	Far,
	UltraFar,
}

public struct NpcTrackedTarget
{
	public ActorEntity Body;
	public Vector3 LastPos;
	public Vector3 LastVel;
	public Vector3 LastAccel;
	public float Danger;
	public bool InWideCone;
	public bool InAimCone;
	public bool TheyFaceMe;
	public float Priority;
	public float Dist;
	public NpcRangeBand Band;
	public float Hp01;
	public float DamageToMe;
	public float DamageToAllies;
	public ulong LastSeenMsec;
	public ulong LastHurtMeMsec;
}

public struct NpcTrackedAlly
{
	public ActorEntity Body;
	public Vector3 Pos;
	public float Dist;
	public float Hp01;
	public int Rank;
	public bool HasStim;
	public bool HasAmmo;
}

/// <summary>
/// Perception board. Wide cone sees. Aim cone = half wide (gun can point).
/// TheyFaceMe = unobstructed and within 45 deg of facing us.
/// </summary>
public sealed class NpcTargetBoard
{
	public readonly List<NpcTrackedTarget> Hostiles = new();
	public readonly List<NpcTrackedAlly> Allies = new();
	public int FocusIndex = -1;

	public NpcTrackedTarget? Focus =>
		FocusIndex >= 0 && FocusIndex < Hostiles.Count ? Hostiles[FocusIndex] : null;

	public static NpcRangeBand BandOf(float dist, float preferred)
	{
		if (preferred < 1f) preferred = 18f;
		var r = dist / preferred;
		if (r < 0.35f) return NpcRangeBand.Point;
		if (r < 0.75f) return NpcRangeBand.Close;
		if (r < 1.25f) return NpcRangeBand.Mid;
		if (r < 2.0f) return NpcRangeBand.Far;
		return NpcRangeBand.UltraFar;
	}

	/// <summary>25% affinity drop per preferred-radius outward. Half that inward.</summary>
	public static float Affinity(float dist, float preferred)
	{
		if (preferred < 1f) preferred = 18f;
		var outR = Mathf.Max(0f, (dist - preferred) / preferred);
		var inR = Mathf.Max(0f, (preferred - dist) / preferred);
		return Mathf.Clamp(1f - 0.25f * outR - 0.125f * inR, 0f, 1f);
	}

	public void NoteDamageFrom(ActorEntity who, float amount, bool toAlly)
	{
		for (int i = 0; i < Hostiles.Count; i++)
		{
			var t = Hostiles[i];
			if (t.Body != who) continue;
			if (toAlly) t.DamageToAllies += amount;
			else { t.DamageToMe += amount; t.LastHurtMeMsec = Time.GetTicksMsec(); }
			t.Priority += amount;
			t.Danger = t.DamageToMe + 0.5f * t.DamageToAllies;
			Hostiles[i] = t;
			return;
		}
	}

	public void Callout(ActorEntity who, Vector3 pos, Vector3 vel)
	{
		for (int i = 0; i < Hostiles.Count; i++)
		{
			var t = Hostiles[i];
			if (t.Body != who) continue;
			t.LastPos = pos;
			t.LastVel = vel;
			t.LastSeenMsec = Time.GetTicksMsec();
			Hostiles[i] = t;
			return;
		}
		Hostiles.Add(new NpcTrackedTarget
		{
			Body = who,
			LastPos = pos,
			LastVel = vel,
			LastSeenMsec = Time.GetTicksMsec(),
			Priority = 1f
		});
	}

	public int PickFocus(float preferred, float gunDamage, bool smart)
	{
		var now = Time.GetTicksMsec();
		var best = -1;
		var bestScore = float.MinValue;
		for (int i = 0; i < Hostiles.Count; i++)
		{
			var t = Hostiles[i];
			if (t.Body == null || t.Body.IsDead) continue;
			var aff = Affinity(t.Dist, preferred);
			var score = t.Priority * aff;
			if (smart)
			{
				var stale = !t.InWideCone && now - t.LastHurtMeMsec > 4000;
				if (stale) score *= 0.35f;
				if (gunDamage > 0.1f && t.Hp01 * 100f < 6.5f * gunDamage)
					score += 50f;
			}
			if (score > bestScore) { bestScore = score; best = i; }
		}
		FocusIndex = best;
		return best;
	}
}

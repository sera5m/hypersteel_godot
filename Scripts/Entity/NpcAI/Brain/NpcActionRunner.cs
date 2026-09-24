using Godot;
using System.Collections.Generic;

namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcActionPhase
{
	Idle = 0,
	Warmup,
	Active,
	CoolOff,
}

/// <summary>Composable. One runner per pawn. Actions are data + a callback.</summary>
public sealed class NpcActionRunner
{
	public NpcActionDef Current { get; private set; }
	public NpcActionPhase Phase { get; private set; }
	public float PhaseLeft { get; private set; }
	public float CooldownLeft { get; private set; }

	readonly Dictionary<string, float> _cd = new();

	public bool Busy => Phase is NpcActionPhase.Warmup or NpcActionPhase.Active;

	public bool TryStart(NpcActionDef def)
	{
		if (def == null) return false;
		if (_cd.TryGetValue(def.Id, out var left) && left > 0f) return false;
		if (Busy && !(Current?.CanCancel ?? true)) return false;
		if (Busy && Current != null)
			Cancel();
		Current = def;
		CooldownLeft = def.Cooldown;
		if (def.Warmup > 0f)
		{
			Phase = NpcActionPhase.Warmup;
			PhaseLeft = def.Warmup + def.InitCost;
		}
		else
		{
			Phase = NpcActionPhase.Active;
			PhaseLeft = def.Duration;
		}
		return true;
	}

	public void Cancel()
	{
		if (Current == null || Phase == NpcActionPhase.Idle) return;
		if (!Current.CanCancel) return;
		StampCd(Current.Cooldown + Current.CancelCost);
		Current = null;
		Phase = NpcActionPhase.Idle;
		PhaseLeft = 0f;
	}

	public bool Tick(float dt)
	{
		foreach (var key in new List<string>(_cd.Keys))
		{
			_cd[key] -= dt;
			if (_cd[key] <= 0f) _cd.Remove(key);
		}

		if (Current == null) return false;
		PhaseLeft -= dt;
		if (PhaseLeft > 0f) return Phase == NpcActionPhase.Active;

		switch (Phase)
		{
			case NpcActionPhase.Warmup:
				Phase = NpcActionPhase.Active;
				PhaseLeft = Current.Duration;
				return true;
			case NpcActionPhase.Active:
				if (Current.CoolOff > 0f)
				{
					Phase = NpcActionPhase.CoolOff;
					PhaseLeft = Current.CoolOff;
					return false;
				}
				Finish();
				return false;
			case NpcActionPhase.CoolOff:
				Finish();
				return false;
		}
		return false;
	}

	public float TimeLeftOn(string id) => _cd.TryGetValue(id, out var v) ? Mathf.Max(0f, v) : 0f;

	void Finish()
	{
		if (Current != null) StampCd(Current.Cooldown);
		Current = null;
		Phase = NpcActionPhase.Idle;
		PhaseLeft = 0f;
	}

	void StampCd(float seconds)
	{
		if (Current == null || seconds <= 0f) return;
		_cd[Current.Id] = seconds;
	}
}

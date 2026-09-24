using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcMoraleState
{
	Neutral = 0,
	Terrified,
	Enraged,
	Joy,
}

/// <summary>Morale ≠ HP. Glory-kill in sight and officer death write this, not packets.</summary>
public sealed class NpcMorale
{
	public NpcMoraleState State { get; private set; }
	public float Left;

	public void Tick(float dt)
	{
		if (Left <= 0f)
		{
			State = NpcMoraleState.Neutral;
			return;
		}
		Left -= dt;
		if (Left <= 0f) State = NpcMoraleState.Neutral;
	}

	public void Set(NpcMoraleState state, float seconds)
	{
		State = state;
		Left = seconds;
	}

	public void OnAllyBrutalized(float fearResist)
	{
		if (GD.Randf() > fearResist)
			Set(NpcMoraleState.Terrified, 4f);
		else
			Set(NpcMoraleState.Enraged, 3f);
	}

	public void OnKill() => Set(NpcMoraleState.Joy, 1.2f);

	public void OnOfficerDied() => Set(NpcMoraleState.Terrified, 5f);
}

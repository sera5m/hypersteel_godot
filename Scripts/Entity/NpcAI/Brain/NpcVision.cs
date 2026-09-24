using Godot;

namespace Hypersteel.Entity.NpcAI.Brain;

public sealed class NpcVision
{
	public bool HasStimulus { get; private set; }
	public float ReactionLeft { get; private set; }
	public Vector3 LastKnown;
	public Vector3 LastVel;
	public bool TheySeeMe;

	public void NotifySeen(NpcStats stats)
	{
		if (HasStimulus) return;
		HasStimulus = true;
		ReactionLeft = stats != null ? stats.Reaction : 0.28f;
	}

	public void NotifyLost()
	{
		HasStimulus = false;
		ReactionLeft = 0f;
	}

	public void Tick(float dt)
	{
		if (ReactionLeft > 0f)
			ReactionLeft -= dt;
	}

	public bool ReactionReady => HasStimulus && ReactionLeft <= 0f;

	public static bool InCone(Vector3 look, Vector3 toTarget, float coneDeg)
	{
		look.Y = 0f;
		toTarget.Y = 0f;
		if (look.LengthSquared() < 1e-6f || toTarget.LengthSquared() < 1e-6f) return false;
		return Mathf.RadToDeg(look.AngleTo(toTarget)) <= coneDeg * 0.5f;
	}

	/// <summary>30 deg slack = fire without body turn. Else need tTurn at max omega.</summary>
	public static bool CanFireWithoutTurn(Vector3 look, Vector3 toAim, float slackDeg)
	{
		return InCone(look, toAim, slackDeg);
	}

	public static float TurnTime(Vector3 look, Vector3 toAim, float slackDeg, float turnRateDeg)
	{
		look.Y = 0f;
		toAim.Y = 0f;
		if (look.LengthSquared() < 1e-6f || toAim.LengthSquared() < 1e-6f) return 0f;
		var theta = Mathf.RadToDeg(look.AngleTo(toAim));
		var extra = theta - slackDeg * 0.5f;
		if (extra <= 0f || turnRateDeg <= 1f) return 0f;
		return extra / turnRateDeg;
	}
}

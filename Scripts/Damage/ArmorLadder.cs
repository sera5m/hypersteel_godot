using System;

namespace Hypersteel.Damage;

public static class ArmorLadder
{
	public static ApOutcome Outcome(int ap, int armor, KineticFlags flags)
	{
		int margin = armor - ap;
		if (margin >= 8) return ApOutcome.Null;
		if ((flags & KineticFlags.Hypervelocity) != 0 && ap >= armor + 6)
			return ApOutcome.Cavitation;
		if (ap >= armor + 3) return ApOutcome.Overpen;
		if (Math.Abs(ap - armor) <= 2) return ApOutcome.Damage;
		if (margin >= 4) return ApOutcome.Bounce;
		return ApOutcome.Damage;
	}

	public static float KineticMul(ApOutcome outcome, int ap, int armor)
	{
		switch (outcome)
		{
			case ApOutcome.Null: return 0f;
			case ApOutcome.Damage: return 1f;
			case ApOutcome.Cavitation: return 1f;
			case ApOutcome.Overpen:
			{
				float mul = 1.30f - 0.10f * (ap - (armor + 3));
				return Math.Max(0.1f, mul);
			}
			case ApOutcome.Bounce:
			{
				// 20% at the Bounce edge (armor-4), +5% per level closer to Damage.
				int below = armor - ap;
				int closer = Math.Max(0, 8 - below);
				return Math.Max(0f, 0.20f + 0.05f * (4 - (below - 4)));
			}
			default: return 1f;
		}
	}
}

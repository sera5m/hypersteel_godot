using System;

namespace Hypersteel.Health;

public static class HitCalculations
{
	public readonly struct Result
	{
		public readonly float Raw;
		public readonly float AfterArmor;
		public readonly float IntoShield;
		public readonly float IntoHp;
		public readonly bool Lethal;

		public Result(float raw, float afterArmor, float intoShield, float intoHp, bool lethal)
		{
			Raw = raw;
			AfterArmor = afterArmor;
			IntoShield = intoShield;
			IntoHp = intoHp;
			Lethal = lethal;
		}
	}

	public static Result Resolve(in Hit hit, HealthRuleConfs rules, BodySegment segment, float hp, float shield)
	{
		float mul = rules.MultiplierFor(segment, hit.Kind);
		float raw = Math.Max(0f, hit.Amount * mul);

		if ((hit.Kind & DamageKind.True) != 0)
			return new Result(raw, raw, 0f, raw, hp - raw <= 0f);

		float afterArmor = raw * (1f - rules.ArmorFor(segment));
		float intoShield = 0f;
		float intoHp = afterArmor;

		if (shield > 0f)
		{
			intoShield = Math.Min(shield, afterArmor);
			intoHp = afterArmor - intoShield;
		}

		return new Result(raw, afterArmor, intoShield, intoHp, hp - intoHp <= 0f);
	}
}

namespace Hypersteel.Damage;

public enum ComboResult
{
	None = 0,
	Detonate,
	Swell,
	MidairExplode,
}

/// <summary>Closed table. Incoming kind vs victim projectile kind.</summary>
public static class ProjectileCombo
{
	public static ComboResult Resolve(ProjectileClass incoming, ProjectileClass victim)
	{
		switch (victim)
		{
			case ProjectileClass.Grenade:
				if (incoming is ProjectileClass.Ballistic or ProjectileClass.Hitscan or ProjectileClass.Lightning)
					return ComboResult.Detonate;
				break;
			case ProjectileClass.Plasma:
				if (incoming == ProjectileClass.Lightning)
					return ComboResult.Swell;
				break;
			case ProjectileClass.Missile:
				if (incoming is ProjectileClass.Lightning or ProjectileClass.Hitscan or ProjectileClass.Ballistic)
					return ComboResult.MidairExplode;
				break;
		}
		return ComboResult.None;
	}
}

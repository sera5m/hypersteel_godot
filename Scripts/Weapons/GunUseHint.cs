using Godot;

namespace Hypersteel.Weapons;

public enum GunRangeFit
{
	Point = 0,
	Close,
	Mid,
	Far,
	Any,
}

public enum GunPlateFit
{
	Soft = 0,
	Any,
	Hull,
}

public enum GunCrowdFit
{
	Single = 0,
	Swarm,
	Any,
}

/// <summary>
/// Authored on the gun. Not an AI-only table.
/// Shotgun: Close + Soft + Swarm. Rifle: Mid, Far if ADS.
/// </summary>
[GlobalClass]
public partial class GunUseHint : Resource
{
	[Export] public GunRangeFit RangeFit = GunRangeFit.Mid;
	[Export] public GunPlateFit PlateFit = GunPlateFit.Any;
	[Export] public GunCrowdFit CrowdFit = GunCrowdFit.Any;
	[Export] public bool WantsAdsAtFar = true;
	[Export] public float BestMeters = 18f;
	[Export] public float OkMeters = 40f;

	public float Score(float dist, bool targetSoft, bool manyNear, bool ads)
	{
		var s = 1f;
		var band = dist <= BestMeters * 0.5f ? GunRangeFit.Point
			: dist <= BestMeters ? GunRangeFit.Close
			: dist <= OkMeters ? GunRangeFit.Mid
			: GunRangeFit.Far;
		if (RangeFit != GunRangeFit.Any && RangeFit != band)
			s -= 0.45f;
		if (PlateFit == GunPlateFit.Soft && !targetSoft) s -= 0.35f;
		if (PlateFit == GunPlateFit.Hull && targetSoft) s -= 0.25f;
		if (CrowdFit == GunCrowdFit.Swarm && !manyNear) s -= 0.2f;
		if (CrowdFit == GunCrowdFit.Single && manyNear) s -= 0.1f;
		if (WantsAdsAtFar && band == GunRangeFit.Far && !ads) s -= 0.15f;
		return s;
	}
}

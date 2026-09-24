using Godot;
using Hypersteel.Weapons;

namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcGunSlot
{
	Primary = 0,
	Sidearm,
	Heavy,
}

public sealed class NpcLoadout
{
	public TestRifle Primary;
	public TestRifle Sidearm;
	public TestRifle Heavy;
	public NpcGunSlot Slot;

	public TestRifle Gun => Slot switch
	{
		NpcGunSlot.Sidearm => Sidearm ?? Primary,
		NpcGunSlot.Heavy => Heavy ?? Primary,
		_ => Primary,
	};

	public void Bind(Node body)
	{
		Primary = body.GetNodeOrNull<TestRifle>("TestRifle")
		          ?? body.GetNodeOrNull<TestRifle>("Primary");
		Sidearm = body.GetNodeOrNull<TestRifle>("Sidearm");
		Heavy = body.GetNodeOrNull<TestRifle>("Heavy");
		if (Primary != null || body is not Node n) return;
		Primary = new TestRifle { Name = "TestRifle" };
		n.AddChild(Primary);
	}

	public bool NeedsReload => Gun != null && !Gun.CheckAmmo() && !Gun.Reloading;

	public void PickSlot(float distance, float close, float far, bool targetIsHull)
	{
		var soft = !targetIsHull;
		var many = false;
		var best = NpcGunSlot.Primary;
		var bestS = Score(Primary, distance, soft, many);

		var side = Score(Sidearm, distance, soft, many);
		if (side > bestS) { bestS = side; best = NpcGunSlot.Sidearm; }

		var heavy = Score(Heavy, distance, soft, many);
		if (heavy > bestS) { best = NpcGunSlot.Heavy; }

		if (bestS < -0.5f)
		{
			if (targetIsHull && Heavy != null) best = NpcGunSlot.Heavy;
			else if (distance <= close && Sidearm != null) best = NpcGunSlot.Sidearm;
			else if (distance >= far && Heavy != null) best = NpcGunSlot.Heavy;
			else best = NpcGunSlot.Primary;
		}

		Slot = best;
	}

	static float Score(TestRifle gun, float dist, bool soft, bool many)
	{
		if (gun == null || !gun.CheckAmmo() && !gun.Reloading) return float.NegativeInfinity;
		if (gun.Use == null) return 0f;
		return gun.Use.Score(dist, soft, many, gun.Ads);
	}
}

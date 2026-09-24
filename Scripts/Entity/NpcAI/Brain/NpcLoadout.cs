using Godot;
using Hypersteel.Weapons;

namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcGunSlot
{
	Primary = 0,
	Sidearm,
	Heavy,
}

/// <summary>Three slots. Active gun is what TryShoot pulls. Not a second inventory.</summary>
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
		if (targetIsHull && Heavy != null) { Slot = NpcGunSlot.Heavy; return; }
		if (distance <= close && Sidearm != null) { Slot = NpcGunSlot.Sidearm; return; }
		if (distance >= far && Heavy != null) { Slot = NpcGunSlot.Heavy; return; }
		Slot = NpcGunSlot.Primary;
	}
}

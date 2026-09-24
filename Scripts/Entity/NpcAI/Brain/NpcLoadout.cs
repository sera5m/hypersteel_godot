using Godot;
using Hypersteel.Weapons;

namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>What this pawn can see of self / squad gear. Not a second inventory.</summary>
public sealed class NpcLoadout
{
	public TestRifle Gun;

	public void Bind(Node body)
	{
		Gun = body.GetNodeOrNull<TestRifle>("TestRifle");
		if (Gun != null || body is not Node n) return;
		Gun = new TestRifle { Name = "TestRifle" };
		n.AddChild(Gun);
	}

	public bool NeedsReload => Gun != null && !Gun.CheckAmmo() && !Gun.Reloading;
}

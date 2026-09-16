using Godot;

namespace Hypersteel.Abilities.Kits;

/// <summary>Data only. Variants are sheets, not subclasses.</summary>
[GlobalClass]
public partial class JumpKitDef : Resource
{
	[Export] public string kitName = "plain";

	[ExportGroup("Bar")]
	[Export] public float capacity = 3f;
	[Export] public float startFilled = 3f;
	[Export] public float passiveRegenPerSec = 0.5f;
	[Export] public float wallrunRegenPerSec = 0.28f;
	[Export] public float landRefundIfBelow = 2f;
	[Export] public float landRefundAmount = 2f;
	[Export] public bool rechargeOnHook;
	[Export] public bool usesFuel = true;

	[ExportGroup("Verbs")]
	[Export] public bool hasAirJump = true;
	[Export] public bool hasDash;
	[Export] public bool hasHover;
	[Export] public bool hasSlam;
	[Export] public bool hasFlight;

	[ExportGroup("Costs")]
	[Export] public float airJumpCost = 1f;
	[Export] public float dashCost = 1.5f;
	[Export] public float hoverDrainPerSec = 1.5f;
	[Export] public float flightDrainPerSec = 0.8f;
	[Export] public bool slamDumpsBar = true;

	[ExportGroup("Motion")]
	[Export] public float airJumpSpeed = 6f;
	[Export] public float dashSpeed = 60f;
	[Export] public float dashSeconds = 0.2f;
	[Export] public float slamSpeed = 40f;
	[Export] public float hoverGravityScale = 0.05f;
	[Export] public float flightGravityScale = 0.15f;
}

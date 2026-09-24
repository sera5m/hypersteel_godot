namespace Hypersteel.Entity.NpcAI.Brain;

public enum NpcVerb
{
	None = 0,
	MoveAndAttack,
	BackUp,
	GetOutOfView,
	Hold,
	Enclose,
	GoLook,
	ShootWorld,
	PulseKit,
}

public enum NpcMacroIntent
{
	None = 0,
	HoldLane,
	Enclose,
	GoLook,
	FallBack,
}

public enum NpcReflexKind
{
	None = 0,
	NadeLeap,
	Dodge,
	Counter,
	Roll,
}

public struct NpcSenseSnapshot
{
	public float OccupantHp01;
	public float PlateBudget01;
	public bool HasFrag;
	public bool HasStim;
	public NpcMacroIntent SuggestedMacro;
	public Godot.Vector3 LastKnownTarget;
	public bool HasLastKnown;
	public bool WorldHazardAhead;
	public Godot.Vector3 WorldHazardPoint;
	public bool HasCover;
	public Godot.Vector3 CoverPoint;
	public bool HasPortal;
	public Godot.Vector3 PortalPoint;
	public bool HasFanDest;
	public Godot.Vector3 FanDest;
}

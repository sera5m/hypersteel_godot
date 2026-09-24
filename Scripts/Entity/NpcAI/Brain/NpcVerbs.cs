namespace Hypersteel.Entity.NpcAI.Brain;

/// <summary>Generic verbs the whole AI system knows. RoleDef only reshapes them.</summary>
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

/// <summary>Coarse team intent. Independent roles ignore this until mesh exists.</summary>
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
}

/// <summary>One-tick sense. Morale is not Health.</summary>
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
}

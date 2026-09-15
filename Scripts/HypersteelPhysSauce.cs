using Godot;
using System;

[GlobalClass]
public partial class HypersteelPhysSauce : Resource
{
	[ExportGroup("Lurch")]
	[Export] public bool lurchEnabled = true;
	[Export(PropertyHint.Range, "0,1")] public float jumpLurch = 0.20f;
	[Export(PropertyHint.Range, "0,1")] public float bhopLurch = 0.15f;

	[ExportGroup("Pivot")]
	[Export] public bool pivotEnabled = true;
	[Export] public float pivotScalar = 0.08f;
	[Export(PropertyHint.Range, "0,1")] public float pivotMax = 0.35f;

	[ExportGroup("Rebound")]
	[Export] public bool reboundEnabled = true;
	[Export(PropertyHint.Range, "0,0.9")] public float reboundMax = 0.90f;
	[Export] public float reboundMinImpact = 6.0f;
	[Export(PropertyHint.Range, "0,1")] public float reboundSteepBounce = 0.65f;
	[Export(PropertyHint.Range, "0,1")] public float reboundRestitution = 0.55f;

	[ExportGroup("Recoil / Shockwave")]
	[Export] public bool recoilEnabled = true;
	[Export] public float punchRecoil = 4.0f;
	[Export] public float fireRecoilScale = 1.0f;
	[Export] public float shockwaveScale = 1.0f;
	[Export] public bool recoilSkeletonOnly = true;

	[ExportGroup("Ground friction / bhop")]
	[Export] public bool highSpeedFrictionCut = true;
	[Export] public float highVelocity = 14.0f;
	[Export] public float highVelFrictionMul = 0.08f;
	[Export] public float frictionRampTime = 0.45f;
	[Export] public bool bhopEnabled = true;
	[Export] public float bhopFrictionIgnore = 0.15f;

	[ExportGroup("Coyote")]
	[Export] public float coyoteTime = 0.18f;
	[Export] public bool coyoteAffectsJump = true;
	[Export] public bool coyoteAffectsFriction = false;

	[ExportGroup("Air drag")]
	[Export] public bool airDragEnabled = true;
	[Export(PropertyHint.Range, "0.05,1")] public float airFrictionSprint = 0.85f;
	[Export(PropertyHint.Range, "0.05,1")] public float airFrictionSlide = 0.40f;

	[ExportGroup("Impact splash (later)")]
	[Export] public bool impactSplashEnabled = false;
	[Export] public float impactSplashMinSpeed = 18.0f;
}

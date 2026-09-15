using Godot;
using Godot.Collections;

namespace Hypersteel.Health;

[GlobalClass]
public partial class HealthRuleConfs : Resource
{
	[Export] public string entityType = "pawn";
	[Export] public float maxHp = 100f;
	[Export] public float maxShield = 0f;
	[Export] public float iFrame = 0.08f;
	[Export] public BodySegment defaultSegment = BodySegment.Torso;

	[Export] public float torsoHp = 100f;
	[Export] public float headHp = 35f;
	[Export] public float armHp = 40f;
	[Export] public float handHp = 15f;
	[Export] public float legHp = 45f;

	[Export(PropertyHint.Range, "0,1")] public float torsoArmor = 0.1f;
	[Export(PropertyHint.Range, "0,1")] public float headArmor = 0f;
	[Export(PropertyHint.Range, "0,1")] public float limbArmor = 0.05f;

	[Export] public float headDamageMul = 1.8f;
	[Export] public float limbDamageMul = 0.85f;
	[Export] public float plasmaMul = 1f;
	[Export] public float kineticMul = 1f;
	[Export] public float microwaveMul = 1f;
	[Export] public float explosiveMul = 1.2f;

	[Export] public Array<BoneSegmentBind> boneMap = new();

	public float MaxFor(BodySegment s) => s switch
	{
		BodySegment.Head => headHp,
		BodySegment.ArmL or BodySegment.ArmR => armHp,
		BodySegment.HandL or BodySegment.HandR => handHp,
		BodySegment.LegL or BodySegment.LegR => legHp,
		_ => torsoHp,
	};

	public float ArmorFor(BodySegment s) => s switch
	{
		BodySegment.Head => headArmor,
		BodySegment.Torso => torsoArmor,
		BodySegment.Generic => torsoArmor,
		_ => limbArmor,
	};

	public float MultiplierFor(BodySegment s, DamageKind kind)
	{
		float loc = s == BodySegment.Head ? headDamageMul
			: (s == BodySegment.Torso || s == BodySegment.Generic) ? 1f
			: limbDamageMul;

		float type = 1f;
		if ((kind & DamageKind.Plasma) != 0) type *= plasmaMul;
		if ((kind & DamageKind.Kinetic) != 0) type *= kineticMul;
		if ((kind & DamageKind.Microwave) != 0) type *= microwaveMul;
		if ((kind & DamageKind.Explosive) != 0) type *= explosiveMul;
		return loc * type;
	}

	public BodySegment SegmentForBone(string boneName)
	{
		if (string.IsNullOrEmpty(boneName) || boneMap == null)
			return defaultSegment;

		foreach (BoneSegmentBind bind in boneMap)
		{
			if (bind == null || string.IsNullOrEmpty(bind.bone)) continue;
			if (boneName == bind.bone || boneName.EndsWith(bind.bone))
				return bind.segment;
		}
		return GuessSegment(boneName);
	}

	public static BodySegment GuessSegment(string boneName)
	{
		string n = boneName.ToLowerInvariant();
		if (n.Contains("head") || n.Contains("neck") || n.Contains("skull")) return BodySegment.Head;
		if (n.Contains("hand") || n.Contains("wrist") || n.Contains("finger") || n.Contains("thumb"))
			return n.Contains("r") && !n.Contains("l") ? BodySegment.HandR : n.Contains("left") || n.Contains(".l") || n.EndsWith("_l") ? BodySegment.HandL : BodySegment.HandR;
		if (n.Contains("arm") || n.Contains("shoulder") || n.Contains("clav") || n.Contains("elbow"))
		{
			bool left = n.Contains("left") || n.Contains(".l") || n.EndsWith("_l") || n.Contains("_l_");
			return left ? BodySegment.ArmL : BodySegment.ArmR;
		}
		if (n.Contains("leg") || n.Contains("thigh") || n.Contains("calf") || n.Contains("foot") || n.Contains("knee") || n.Contains("hip"))
		{
			bool left = n.Contains("left") || n.Contains(".l") || n.EndsWith("_l") || n.Contains("_l_");
			return left ? BodySegment.LegL : BodySegment.LegR;
		}
		return BodySegment.Torso;
	}
}

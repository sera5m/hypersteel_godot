using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

[System.Flags]
public enum ArmorAttr
{
	None = 0,
	MultiLayerComposite = 1 << 0,
	SolidMetal = 1 << 1,
	PlasmaShield = 1 << 2,
	Mirror = 1 << 3,
	ShearThickPadded = 1 << 4,
	NanotubeFiber = 1 << 5,
	CeramicPlate = 1 << 6,
	Flesh = 1 << 7,
	FleshBone = 1 << 8,
	FleshBorged = 1 << 9,
	MetalFoamPadded = 1 << 10,
}

[GlobalClass]
public partial class ArmorPlate : Resource
{
	[Export] public string plateName = "plate";
	[Export] public BodySegment segment = BodySegment.Torso;
	[Export] public int armorLevel = 3;
	[Export] public float plateHp = 40f;
	[Export] public ArmorAttr attributes;
	[Export] public ArmorAttr attributesSecond;
	[Export] public bool inherent;
}

[GlobalClass]
public partial class ArmorSheet : Resource
{
	[Export] public Godot.Collections.Array<ArmorPlate> plates = new();
	[Export] public float resistKinetic = 1f;
	[Export] public float resistThermal = 1f;
	[Export] public float resistIon = 1f;
	[Export] public float resistToxic = 1f;
}

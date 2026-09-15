using Godot;

namespace Hypersteel.Health;

[GlobalClass]
public partial class BoneSegmentBind : Resource
{
	[Export] public string bone = "";
	[Export] public BodySegment segment = BodySegment.Torso;
}

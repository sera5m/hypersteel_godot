using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

public partial class HurtBox : Area3D
{
	[Export] public BodySegment segment = BodySegment.Torso;
	[Export] public StringName bone;
	[Export] public bool isPlate;

	public override void _Ready()
	{
		CollisionLayer |= CollisionLayers.Damage;
		CollisionMask = 0;
		Monitorable = true;
		Monitoring = false;
	}

	public void Receive(DamagePacket packet)
	{
		packet.Segment = segment;
		if (bone.IsEmpty == false)
			packet.Bone = bone;
		DamageProbe.TryHurt(this, packet);
	}
}

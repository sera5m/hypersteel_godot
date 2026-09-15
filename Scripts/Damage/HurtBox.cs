using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

/// <summary>
/// Put on a limb collider. Bullet hits this node; we walk to ActorEntity.
/// </summary>
public partial class HurtBox : Area3D
{
	[Export] public BodySegment segment = BodySegment.Torso;
	[Export] public StringName bone;
	[Export] public bool isPlate;

	public void Receive(DamagePacket packet)
	{
		packet.Segment = segment;
		if (bone.IsEmpty == false)
			packet.Bone = bone;
		DamageProbe.TryHurt(this, packet);
	}
}

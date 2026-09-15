using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

/// <summary>Armor in reverse. Inside the body. Multiplies through-damage and stamps a tag.</summary>
public partial class CritSocket : Node3D, IDamageable
{
	[Export] public BodySegment segment = BodySegment.Head;
	[Export] public float damageMul = 1.8f;
	[Export] public StatusTag applyTag;
	[Export] public bool lethalOnPierce;

	[Signal] public delegate void CritHitEventHandler(float taken, Node3D cause);

	public bool IsDead => false;

	public void Hurt(DamagePacket packet)
	{
		packet.Segment = segment;
		packet.Kinetic *= damageMul;
		packet.FromCover = true;
		packet.Cover = this;
		EmitSignal(SignalName.CritHit, packet.Kinetic, packet.Instigator);

		var actor = DamageProbe.FindEntity(this);
		if (actor == null) return;
		if (lethalOnPierce && (packet.KineticFlags & KineticFlags.Piercing) != 0 && packet.Ap >= 3)
			packet.Kinetic = Math.Max(packet.Kinetic, 99999f);
		actor.Hurt(packet);
		if (applyTag != StatusTag.None && actor.status != null)
			actor.status.Add(applyTag);
	}
}

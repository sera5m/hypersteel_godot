using Godot;
using Hypersteel.Entity;

namespace Hypersteel.Damage;

/// <summary>Reads Jolt/Godot contacts, writes blunt packets. Does not replace physics.</summary>
public partial class CollisionDamage : Node
{
	[Export] public float Mass = 80f;
	[Export] public float MinKinetic = 6f;
	[Export] public float CooldownSec = 0.12f;
	[Export] public bool enabled = true;

	CharacterBody3D _body;
	ulong _ignoreUntil;

	public override void _Ready()
	{
		_body = GetParent() as CharacterBody3D ?? GetOwner() as CharacterBody3D;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!enabled || _body == null) return;
		int n = _body.GetSlideCollisionCount();
		for (int i = 0; i < n; i++)
			Consider(_body.GetSlideCollision(i));
	}

	public void Consider(KinematicCollision3D hit)
	{
		if (hit == null) return;
		ulong now = Time.GetTicksMsec();
		if (now < _ignoreUntil) return;

		Node other = hit.GetCollider() as Node;
		if (other == null) return;

		IDamageable self = DamageProbe.FindDamageable(_body);
		IDamageable victim = DamageProbe.FindDamageable(other);
		if (self == null) return;

		Vector3 nrm = hit.GetNormal();
		Vector3 rel = _body.Velocity;
		if (other is CharacterBody3D cb) rel -= cb.Velocity;
		else if (other is RigidBody3D rb) rel -= rb.LinearVelocity;

		float glance = CollisionImpact.Glancing01(rel, nrm);
		float along = rel.Dot(nrm);
		float myMass = CollisionImpact.MassOf(_body, Mass);
		float theirMass = CollisionImpact.MassOf(other, Mass);
		float kinetic = CollisionImpact.Kinetic(myMass, theirMass, along, glance);
		if (kinetic < MinKinetic) return;

		_ignoreUntil = now + (ulong)(CooldownSec * 1000f);

		var packet = DamagePacket.KineticHit(kinetic, ap: 1, DamageSource.Contact);
		packet.KineticFlags = KineticFlags.Blunt;
		packet.Point = hit.GetPosition();
		packet.Normal = nrm;
		packet.Instigator = _body;

		// Heavier object crushes the lighter IDamageable. If both are actors, each side that has this component will fire once per cooldown.
		if (victim != null && victim != self && theirMass >= myMass * 0.25f)
			victim.Hurt(packet);
		else if (theirMass / myMass >= CollisionImpact.CrushMassRatio)
			self.Hurt(packet);
	}
}

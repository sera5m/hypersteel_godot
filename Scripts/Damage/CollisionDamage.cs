using Godot;

namespace Hypersteel.Damage;

/// <summary>
/// After Jolt resolves the shove, this body may take blunt once.
/// Never writes damage onto the other actor — that stops the GMod double-hit.
/// </summary>
public partial class CollisionDamage : Node
{
	[Export] public bool DoesBluntForce = true;
	[Export] public float Mass = 80f;
	[Export] public float Restitution = CollisionImpact.FleshRestitution;
	[Export] public float Footing = 1f;
	[Export] public float ResistBlunt = 0f;
	[Export] public float MinKinetic = 6f;
	[Export] public float FlinchAt = 18f;
	[Export] public float RagdollAt = 70f;
	[Export] public float KnockOffAt = 22f;
	[Export] public float CooldownSec = 0.15f;
	[Export] public MotionStance Stance = MotionStance.Ground;

	public bool LastKnockOff { get; private set; }
	public bool LastRagdoll { get; private set; }

	[Signal] public delegate void BluntFlinchEventHandler(float intensity);
	[Signal] public delegate void BluntRagdollEventHandler(float intensity);
	[Signal] public delegate void BluntKnockOffEventHandler();

	CharacterBody3D _body;
	ulong _ignoreUntil;

	public override void _Ready()
	{
		_body = GetParent() as CharacterBody3D ?? GetOwner() as CharacterBody3D;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!DoesBluntForce || _body == null) return;
		LastKnockOff = false;
		LastRagdoll = false;
		InferStance();
		int n = _body.GetSlideCollisionCount();
		for (int i = 0; i < n; i++)
			Consider(_body.GetSlideCollision(i));
	}

	public void SetStance(MotionStance s) => Stance = s;

	void InferStance()
	{
		// FSM should call SetStance. Fallback if it has not.
		if (Stance is MotionStance.Climb or MotionStance.Wallrun or MotionStance.Crouch or MotionStance.Prone)
			return;
		Stance = _body.IsOnFloor() ? MotionStance.Ground : MotionStance.Air;
	}

	void Consider(KinematicCollision3D hit)
	{
		if (hit == null) return;
		ulong now = Time.GetTicksMsec();
		if (now < _ignoreUntil) return;

		Node other = hit.GetCollider() as Node;
		if (other == null) return;

		IDamageable self = DamageProbe.FindDamageable(_body);
		if (self == null) return;

		Vector3 nrm = hit.GetNormal();
		Vector3 rel = _body.Velocity;
		if (other is CharacterBody3D cb) rel -= cb.Velocity;
		else if (other is RigidBody3D rb) rel -= rb.LinearVelocity;

		float myMass = CollisionImpact.MassOf(_body, Mass);
		float theirMass = CollisionImpact.MassOf(other, Mass);
		float e = Math.Max(Restitution, CollisionImpact.RestitutionOf(other));
		float raw = CollisionImpact.Kinetic(myMass, theirMass, rel, nrm, e);
		float normal01 = CollisionImpact.NormalEnergy01(rel, nrm);
		float stance = CollisionImpact.StanceMul(Stance);
		float resist = Math.Clamp(ResistBlunt, 0f, 0.95f);
		float kinetic = raw * normal01 * stance * Footing * (1f - resist);
		if (kinetic < MinKinetic) return;

		_ignoreUntil = now + (ulong)(CooldownSec * 1000f);

		var packet = DamagePacket.KineticHit(kinetic, ap: 1, DamageSource.Contact);
		packet.KineticFlags = KineticFlags.Blunt;
		packet.Point = hit.GetPosition();
		packet.Normal = nrm;
		packet.Instigator = other as Node3D ?? _body;
		self.Hurt(packet);

		if (kinetic >= RagdollAt)
		{
			LastRagdoll = true;
			EmitSignal(SignalName.BluntRagdoll, kinetic);
		}
		else if (kinetic >= FlinchAt)
			EmitSignal(SignalName.BluntFlinch, kinetic / FlinchAt);

		if (Stance == MotionStance.Wallrun && kinetic >= KnockOffAt)
		{
			LastKnockOff = true;
			EmitSignal(SignalName.BluntKnockOff);
		}
	}
}

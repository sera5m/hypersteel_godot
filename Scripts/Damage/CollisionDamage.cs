using System.Collections.Generic;
using Godot;

namespace Hypersteel.Damage;

/// <summary>
/// Watches a contact. Jolt owns the shove. After the pair separates, blunt is
/// computed from measured Δv, once, on this body only.
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
	[Export] public float MaxWatchSec = 0.35f;
	[Export] public MotionStance Stance = MotionStance.Ground;

	public bool LastKnockOff { get; private set; }
	public bool LastRagdoll { get; private set; }

	[Signal] public delegate void BluntFlinchEventHandler(float intensity);
	[Signal] public delegate void BluntRagdollEventHandler(float intensity);
	[Signal] public delegate void BluntKnockOffEventHandler();

	struct Watch
	{
		public Rid Other;
		public Node OtherNode;
		public Vector3 Normal;
		public Vector3 RelBefore;
		public Vector3 RelPeakClose;
		public float TheirMass;
		public float Restitution;
		public ulong StartMsec;
		public bool StillTouching;
	}

	CharacterBody3D _body;
	readonly Dictionary<Rid, Watch> _open = new();

	public override void _Ready()
	{
		_body = GetParent() as CharacterBody3D ?? GetOwner() as CharacterBody3D;
	}

	public void SetStance(MotionStance s) => Stance = s;

	public override void _PhysicsProcess(double delta)
	{
		if (!DoesBluntForce || _body == null) return;
		LastKnockOff = false;
		LastRagdoll = false;
		InferStance();

		MarkUntouched();
		int n = _body.GetSlideCollisionCount();
		for (int i = 0; i < n; i++)
			Touch(_body.GetSlideCollision(i));

		ConcludeSeparated();
	}

	void InferStance()
	{
		if (Stance is MotionStance.Climb or MotionStance.Wallrun or MotionStance.Crouch or MotionStance.Prone)
			return;
		Stance = _body.IsOnFloor() ? MotionStance.Ground : MotionStance.Air;
	}

	void MarkUntouched()
	{
		if (_open.Count == 0) return;
		var keys = new List<Rid>(_open.Keys);
		foreach (Rid id in keys)
		{
			Watch w = _open[id];
			w.StillTouching = false;
			_open[id] = w;
		}
	}

	void Touch(KinematicCollision3D hit)
	{
		if (hit == null) return;
		Node other = hit.GetCollider() as Node;
		Rid rid = hit.GetColliderRid();
		if (!rid.IsValid || other == null) return;

		Vector3 nrm = hit.GetNormal();
		Vector3 rel = RelVel(other);

		if (_open.TryGetValue(rid, out Watch w))
		{
			w.StillTouching = true;
			w.Normal = nrm;
			if (Closing(rel, nrm) > Closing(w.RelPeakClose, nrm))
				w.RelPeakClose = rel;
			_open[rid] = w;
			return;
		}

		_open[rid] = new Watch
		{
			Other = rid,
			OtherNode = other,
			Normal = nrm,
			RelBefore = rel,
			RelPeakClose = rel,
			TheirMass = CollisionImpact.MassOf(other, Mass),
			Restitution = Math.Max(Restitution, CollisionImpact.RestitutionOf(other)),
			StartMsec = Time.GetTicksMsec(),
			StillTouching = true,
		};
	}

	void ConcludeSeparated()
	{
		if (_open.Count == 0) return;
		ulong now = Time.GetTicksMsec();
		var done = new List<Rid>();
		foreach (KeyValuePair<Rid, Watch> kv in _open)
		{
			Watch w = kv.Value;
			bool timeout = (now - w.StartMsec) > (ulong)(MaxWatchSec * 1000f);
			if (w.StillTouching && !timeout) continue;
			Apply(w);
			done.Add(kv.Key);
		}
		foreach (Rid id in done)
			_open.Remove(id);
	}

	void Apply(Watch w)
	{
		IDamageable self = DamageProbe.FindDamageable(_body);
		if (self == null) return;

		Vector3 relAfter = RelVel(w.OtherNode);
		// Measured change in closing speed — what Jolt actually took out of the pair.
		float closeBefore = Closing(w.RelPeakClose, w.Normal);
		float closeAfter = Closing(relAfter, w.Normal);
		float taken = Math.Max(0f, closeBefore - closeAfter);
		Vector3 measured = w.Normal * taken;

		float raw = CollisionImpact.Kinetic(Mass, w.TheirMass, measured, w.Normal, w.Restitution);
		float normal01 = CollisionImpact.NormalEnergy01(w.RelPeakClose, w.Normal);
		float kinetic = raw * normal01 * CollisionImpact.StanceMul(Stance) * Footing * (1f - Math.Clamp(ResistBlunt, 0f, 0.95f));
		if (kinetic < MinKinetic) return;

		var packet = DamagePacket.KineticHit(kinetic, ap: 1, DamageSource.Contact);
		packet.KineticFlags = KineticFlags.Blunt;
		packet.Normal = w.Normal;
		packet.Instigator = w.OtherNode as Node3D ?? _body;
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

	Vector3 RelVel(Node other)
	{
		Vector3 rel = _body.Velocity;
		if (other is CharacterBody3D cb) rel -= cb.Velocity;
		else if (other is RigidBody3D rb) rel -= rb.LinearVelocity;
		return rel;
	}

	static float Closing(Vector3 rel, Vector3 n)
	{
		Vector3 nn = n.LengthSquared() > 1e-8f ? n.Normalized() : Vector3.Up;
		return Math.Max(0f, -rel.Dot(nn));
	}
}

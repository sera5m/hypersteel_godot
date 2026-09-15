using System.Collections.Generic;
using Godot;

namespace Hypersteel.Damage;

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

	CharacterBody3D _kine;
	RigidBody3D _rigid;
	readonly Dictionary<Rid, Watch> _open = new();

	public override void _Ready()
	{
		_kine = GetParent() as CharacterBody3D ?? GetOwner() as CharacterBody3D;
		_rigid = GetParent() as RigidBody3D ?? GetOwner() as RigidBody3D;
		if (_rigid != null)
		{
			_rigid.ContactMonitor = true;
			_rigid.MaxContactsReported = Math.Max(_rigid.MaxContactsReported, 8);
			_rigid.BodyEntered += OnRigidEnter;
			_rigid.BodyExited += OnRigidExit;
		}
	}

	public void SetStance(MotionStance s) => Stance = s;

	public override void _PhysicsProcess(double delta)
	{
		if (!DoesBluntForce) return;
		LastKnockOff = false;
		LastRagdoll = false;

		if (_kine != null)
		{
			InferStance();
			MarkUntouched();
			int n = _kine.GetSlideCollisionCount();
			for (int i = 0; i < n; i++)
				TouchSlide(_kine.GetSlideCollision(i));
			ConcludeSeparated();
			return;
		}

		if (_rigid != null)
		{
			RefreshRigidPeak();
			ConcludeSeparated();
		}
	}

	void InferStance()
	{
		if (Stance is MotionStance.Climb or MotionStance.Wallrun or MotionStance.Crouch or MotionStance.Prone)
			return;
		Stance = _kine != null && _kine.IsOnFloor() ? MotionStance.Ground : MotionStance.Air;
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

	void TouchSlide(KinematicCollision3D hit)
	{
		if (hit == null) return;
		OpenOrPulse(hit.GetCollider() as Node, hit.GetColliderRid(), hit.GetNormal());
	}

	void OnRigidEnter(Node other)
	{
		if (other is PhysicsBody3D pb)
			OpenOrPulse(other, pb.GetRid(), Vector3.Up);
	}

	void OnRigidExit(Node other)
	{
		if (other is not PhysicsBody3D pb) return;
		if (!_open.TryGetValue(pb.GetRid(), out Watch w)) return;
		w.StillTouching = false;
		_open[pb.GetRid()] = w;
	}

	void RefreshRigidPeak()
	{
		if (_open.Count == 0 || _rigid == null) return;
		var keys = new List<Rid>(_open.Keys);
		foreach (Rid id in keys)
		{
			Watch w = _open[id];
			if (!w.StillTouching) continue;
			Vector3 rel = RelVel(w.OtherNode);
			if (Closing(rel, w.Normal) > Closing(w.RelPeakClose, w.Normal))
				w.RelPeakClose = rel;
			_open[id] = w;
		}
	}

	void OpenOrPulse(Node other, Rid rid, Vector3 nrm)
	{
		if (!rid.IsValid || other == null) return;
		Vector3 rel = RelVel(other);
		if (_open.TryGetValue(rid, out Watch w))
		{
			w.StillTouching = true;
			w.Normal = nrm.LengthSquared() > 1e-8f ? nrm : w.Normal;
			if (Closing(rel, w.Normal) > Closing(w.RelPeakClose, w.Normal))
				w.RelPeakClose = rel;
			_open[rid] = w;
			return;
		}
		_open[rid] = new Watch
		{
			Other = rid,
			OtherNode = other,
			Normal = nrm.LengthSquared() > 1e-8f ? nrm : Vector3.Up,
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
		foreach (Rid id in done) _open.Remove(id);
	}

	void Apply(Watch w)
	{
		Node host = (Node)_kine ?? _rigid;
		IDamageable self = DamageProbe.FindDamageable(host);
		if (self == null) return;

		Vector3 relAfter = RelVel(w.OtherNode);
		float taken = Math.Max(0f, Closing(w.RelPeakClose, w.Normal) - Closing(relAfter, w.Normal));
		Vector3 measured = w.Normal * taken;
		float raw = CollisionImpact.Kinetic(Mass, w.TheirMass, measured, w.Normal, w.Restitution);
		float kinetic = raw * CollisionImpact.NormalEnergy01(w.RelPeakClose, w.Normal)
			* CollisionImpact.StanceMul(Stance) * Footing * (1f - Math.Clamp(ResistBlunt, 0f, 0.95f));
		if (kinetic < MinKinetic) return;

		var packet = DamagePacket.KineticHit(kinetic, ap: 1, DamageSource.Contact);
		packet.KineticFlags = KineticFlags.Blunt;
		packet.Normal = w.Normal;
		packet.Instigator = w.OtherNode as Node3D ?? host as Node3D;
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
		Vector3 mine = _kine != null ? _kine.Velocity : _rigid != null ? _rigid.LinearVelocity : Vector3.Zero;
		if (other is CharacterBody3D cb) return mine - cb.Velocity;
		if (other is RigidBody3D rb) return mine - rb.LinearVelocity;
		return mine;
	}

	static float Closing(Vector3 rel, Vector3 n)
	{
		Vector3 nn = n.LengthSquared() > 1e-8f ? n.Normalized() : Vector3.Up;
		return Math.Max(0f, -rel.Dot(nn));
	}
}

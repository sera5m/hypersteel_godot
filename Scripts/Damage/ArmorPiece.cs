using Godot;
using Hypersteel.Entity;
using Hypersteel.Health;

namespace Hypersteel.Damage;

public partial class ArmorPiece : Node3D, IDamageable
{
	[Export] public ArmorPlate plate;
	[Export] public BodySegment mount = BodySegment.Torso;
	[Export] public Vector3 comAxis = new Vector3(0f, 1f, 0f);
	[Export] public NodePath colliderPath;
	[Export] public bool watchEnabled;
	[Export] public float shatterAtHpFrac = 0.25f;

	public float Hp { get; private set; }
	public float MaxHp { get; private set; }
	public bool Broken { get; private set; }
	public bool Shattered { get; private set; }
	public readonly DamageWatch Watch = new();

	public float LastThrough { get; private set; }
	public bool LastOverpen { get; private set; }
	public Vector3 LastBounce { get; private set; }

	[Signal] public delegate void ArmorDamagedEventHandler(float taken, Node3D cause);
	[Signal] public delegate void ArmorShatteredEventHandler(Node3D cause);
	[Signal] public delegate void ArmorBrokenEventHandler(Node3D cause);
	[Signal] public delegate void ArmorRepairedEventHandler(Node3D cause);
	[Signal] public delegate void ArmorBouncedEventHandler(Vector3 bounceDir, Node3D cause);

	public override void _Ready()
	{
		plate ??= new ArmorPlate { segment = mount, armorLevel = 3, plateHp = 40f };
		mount = plate.segment;
		MaxHp = plate.plateHp;
		Hp = MaxHp;
		Watch.Enabled = watchEnabled;
		StampDamageLayer(this);
		if (colliderPath != null && !colliderPath.IsEmpty)
		{
			var col = GetNodeOrNull<CollisionObject3D>(colliderPath);
			if (col != null) StampDamageLayer(col);
		}
	}

	public bool IsDead => Broken || Hp <= 0f;

	public void Hurt(DamagePacket packet)
	{
		ActorEntity wearer = DamageProbe.FindEntity(this);
		LastThrough = 0f;
		LastOverpen = false;

		if (Broken)
		{
			LastThrough = packet.Kinetic;
			LastOverpen = true;
			Forward(wearer, packet);
			return;
		}

		Vector3 incoming = packet.Normal.LengthSquared() > 0.0001f ? -packet.Normal : -GlobalTransform.Basis.Z;
		var soak = DamageReceive.Armor(packet, plate.armorLevel, Hp, comAxis, packet.Point, incoming);

		Hp = Math.Max(0f, Hp - soak.TakenByCover);
		Watch.Sample(soak.TakenByCover, Time.GetTicksMsec());
		LastThrough = soak.Through;
		LastOverpen = soak.Outcome is ApOutcome.Overpen or ApOutcome.Cavitation || (!soak.Stopped && soak.Through > 0.01f);
		LastBounce = soak.BounceDir;

		Node3D cause = packet.Instigator;
		EmitSignal(SignalName.ArmorDamaged, soak.TakenByCover, cause);
		if (soak.Outcome == ApOutcome.Null || soak.Outcome == ApOutcome.Bounce)
			EmitSignal(SignalName.ArmorBounced, soak.BounceDir, cause);

		if (!Shattered && MaxHp > 0f && Hp / MaxHp <= shatterAtHpFrac)
		{
			Shattered = true;
			plate.armorLevel = Math.Max(0, plate.armorLevel - 2);
			EmitSignal(SignalName.ArmorShattered, cause);
		}

		if (Hp <= 0f && !Broken)
		{
			Broken = true;
			EmitSignal(SignalName.ArmorBroken, cause);
		}

		if (soak.Stopped)
		{
			LastOverpen = false;
			return;
		}

		if (wearer != null && wearer.WantCoverLog)
			packet.RecordCover(this, soak.TakenByCover, soak.Through, mount);

		packet.Kinetic = soak.Through;
		packet.Segment = mount;
		packet.FromCover = true;
		packet.Cover = this;
		Forward(wearer, packet);
	}

	public void Repair(float amount, Node3D cause)
	{
		bool wasBroken = Broken;
		Hp = Math.Min(MaxHp, Hp + amount);
		Broken = Hp <= 0f;
		Shattered = Shattered && Hp / Math.Max(1f, MaxHp) <= shatterAtHpFrac;
		if (wasBroken && !Broken)
			EmitSignal(SignalName.ArmorRepaired, cause);
	}

	public void ConcludeDot() => Watch.ConcludeDot(Time.GetTicksMsec());

	static void Forward(ActorEntity wearer, DamagePacket packet) => wearer?.Hurt(packet);

	static void StampDamageLayer(Node n)
	{
		if (n is CollisionObject3D co)
		{
			co.CollisionLayer |= CollisionLayers.Damage;
			co.CollisionMask = 0;
		}
	}
}

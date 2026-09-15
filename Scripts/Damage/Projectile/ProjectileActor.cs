using Godot;
using Hypersteel.Entity;

namespace Hypersteel.Damage;

/// <summary>Flyer. Shared def pointer. Shot-able so grenades cook off.</summary>
public partial class ProjectileActor : Node3D, IDamageable
{
	[Export] public ProjectileDef def;
	[Export] public Area3D impact;
	[Export] public Area3D nearMiss;
	[Export] public ProjectileMove move;

	public Node3D Shooter;
	public bool IsDead { get; private set; }

	float _age;
	bool _stuck;

	public override void _Ready()
	{
		def ??= new ProjectileDef();
		move ??= GetNodeOrNull<ProjectileMove>("Move");
		if (move != null)
		{
			move.def = def;
			move.Body = this;
			move.Velocity = -GlobalTransform.Basis.Z * def.speed;
		}

		impact ??= GetNodeOrNull<Area3D>("Impact");
		nearMiss ??= GetNodeOrNull<Area3D>("NearMiss");
		Stamp(impact, CollisionLayers.Damage);
		Stamp(nearMiss, CollisionLayers.Damage);
		if (impact != null)
		{
			impact.AreaEntered += OnImpactArea;
			impact.BodyEntered += OnImpactBody;
		}
		if (nearMiss != null)
		{
			nearMiss.AreaEntered += OnNearArea;
			nearMiss.BodyEntered += OnNearBody;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (IsDead || _stuck) return;
		_age += (float)delta;
		if (_age >= def.lifetime)
			Die(ProjectileDeath.Delete, null);
	}

	public void Launch(Node3D shooter, Vector3 origin, Vector3 dir, ProjectileDef shared)
	{
		Shooter = shooter;
		def = shared ?? def;
		GlobalPosition = origin;
		if (dir.LengthSquared() > 0.0001f)
			LookAt(origin + dir, Vector3.Up);
		if (move != null)
		{
			move.def = def;
			move.Velocity = dir.Normalized() * def.speed;
		}
	}

	public void Hurt(DamagePacket packet)
	{
		if (IsDead || def == null) return;
		ProjectileClass incoming = Classify(packet);
		ComboResult combo = ProjectileCombo.Resolve(incoming, def.kind);
		switch (combo)
		{
			case ComboResult.Swell:
				def.kinetic *= 1.35f;
				Scale *= 1.2f;
				return;
			case ComboResult.Detonate:
			case ComboResult.MidairExplode:
				Die(ProjectileDeath.Detonate, packet.Instigator);
				return;
		}
		Die(def.death, packet.Instigator);
	}

	void OnImpactArea(Area3D other) => HitNode(other);
	void OnImpactBody(Node3D other) => HitNode(other);

	void OnNearArea(Area3D other) => Near(other);
	void OnNearBody(Node3D other) => Near(other);

	void HitNode(Node other)
	{
		if (IsDead || other == null) return;
		if (Shooter != null && (other == Shooter || Shooter.IsAncestorOf(other) || other.IsAncestorOf(Shooter)))
			return;

		DamagePacket packet = BuildPacket();
		IDamageable dmg = DamageProbe.FindDamageable(other);
		if (dmg != null && dmg != (IDamageable)this)
		{
			packet.Point = GlobalPosition;
			packet.Normal = -GlobalTransform.Basis.Z;
			dmg.Hurt(packet);
			Spawn(def.spawnOnHit, GlobalPosition, packet.Normal);
			Die(OverpenDeath(packet), Shooter);
			return;
		}

		// World / surface
		Spawn(def.spawnOnWorld ?? def.spawnOnHit, GlobalPosition, packet.Normal);
		if (def.stickOnHit)
		{
			_stuck = true;
			if (move != null) move.Velocity = Vector3.Zero;
			return;
		}
		Die(def.death, Shooter);
	}

	void Near(Node other)
	{
		if (IsDead || other == null) return;
		if (Shooter != null && other == Shooter) return;
		bool wake = def.wakeCavitation || (def.flags & KineticFlags.Hypervelocity) != 0;
		if (wake)
		{
			var p = BuildPacket();
			p.Kinetic *= 0.25f;
			p.Source = DamageSource.Splash;
			DamageProbe.TryHurt(other, p);
			return;
		}
		if (DamageProbe.FindEntity(other) != null)
			EmitWhoosh(other);
	}

	void EmitWhoosh(Node other) => GD.Print($"[proj] whoosh {other.Name}");

	DamagePacket BuildPacket()
	{
		float kinetic = def.kinetic;
		if (def.followRealVel && move != null)
			kinetic = PlaceholderJoules(move.Velocity.Length(), def.crossSectionCm2);
		var p = DamagePacket.KineticHit(kinetic, def.ap, DamageSource.Projectile);
		p.Name = def.defId;
		p.KineticFlags = def.flags;
		if (def.wakeCavitation) p.KineticFlags |= KineticFlags.Hypervelocity;
		p.Instigator = Shooter;
		if (def.shrapnelCount > 0)
		{
			p.HasShrapnel = true;
			p.Shrapnel = new ShrapnelRecipe
			{
				Fragments = def.shrapnelCount,
				SpawnWorldIfExit = def.shrapnelOnExit,
				Piercing = true,
			};
		}
		return p;
	}

	static float PlaceholderJoules(float speed, float cm2)
	{
		// Placeholder. Does not change AP.
		return speed * speed * Math.Max(0.01f, cm2) * 0.01f;
	}

	static ProjectileClass Classify(DamagePacket packet)
	{
		if ((packet.KineticFlags & KineticFlags.Incendiary) != 0) return ProjectileClass.Flame;
		return packet.Source switch
		{
			DamageSource.Beam => ProjectileClass.Hitscan,
			DamageSource.Splash => ProjectileClass.Grenade,
			_ => ProjectileClass.Ballistic,
		};
	}

	ProjectileDeath OverpenDeath(DamagePacket packet)
	{
		if (def.kind == ProjectileClass.Grenade || def.kind == ProjectileClass.Missile)
			return ProjectileDeath.Detonate;
		return def.death;
	}

	void Die(ProjectileDeath how, Node3D cause)
	{
		if (IsDead) return;
		IsDead = true;
		switch (how)
		{
			case ProjectileDeath.Shrapnel:
			{
				var p = BuildPacket();
				p.HasShrapnel = true;
				p.Shrapnel = new ShrapnelRecipe { Fragments = Math.Max(3, def.shrapnelCount), SpawnWorldIfExit = true, Piercing = true };
				DamageCast.Ray(this, GlobalPosition, -GlobalTransform.Basis.Z, 8f, p, GetRidSafe());
				break;
			}
			case ProjectileDeath.Detonate:
			{
				var p = BuildPacket();
				p.Source = DamageSource.Splash;
				p.KineticFlags |= KineticFlags.Blunt;
				// Placeholder blast: tube of leftover energy. Real shockwave later.
				DamageCast.Tube(this, GlobalPosition, Vector3.Up, 2f, p, 1.2f, GetRidSafe());
				break;
			}
		}
		QueueFree();
	}

	Rid GetRidSafe() => impact != null ? impact.GetRid() : default;

	static void Stamp(Area3D area, uint layer)
	{
		if (area == null) return;
		area.CollisionLayer = layer;
		area.CollisionMask = CollisionLayers.Trace;
		area.Monitoring = true;
		area.Monitorable = true;
	}

	static void Spawn(PackedScene scene, Vector3 at, Vector3 normal)
	{
		if (scene == null) return;
		var n = scene.Instantiate<Node3D>();
		n.GlobalPosition = at;
		if (normal.LengthSquared() > 0.0001f)
			n.LookAt(at + normal);
		// Parent later via current scene. Caller should add_child from a spawner.
		n.GetTree()?.CurrentScene?.AddChild(n);
	}
}

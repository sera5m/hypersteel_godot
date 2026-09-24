using Godot;
using Hypersteel.Damage;

namespace Hypersteel.Weapons;

public partial class TestRifle : Gun
{
	[Export] public int MagSize = 30;
	[Export] public int Reserve = 90;
	[Export] public float Muzzle = 80f;
	[Export] public float Kinetic = 40f;
	[Export] public int Ap = 3;
	[Export] public float Range = 80f;
	[Export] public float ReloadTime = 1.6f;
	[Export] public bool Hitscan = true;

	public int InMag { get; private set; }
	public bool Reloading { get; private set; }
	public Node3D Target { get; private set; }

	float _reloadLeft;
	Node3D _owner;

	public override void _Ready()
	{
		base._Ready();
		InMag = MagSize;
		_owner = GetParent() as Node3D;
		Use ??= new GunUseHint { RangeFit = GunRangeFit.Mid, BestMeters = 18f, OkMeters = 55f, WantsAdsAtFar = true };
	}

	public override void _Process(double delta)
	{
		if (!Reloading) return;
		_reloadLeft -= (float)delta;
		if (_reloadLeft > 0f) return;
		var need = MagSize - InMag;
		var take = Mathf.Min(need, Reserve);
		Reserve -= take;
		InMag += take;
		Reloading = false;
	}

	public void SelectTarget(Node3D t) => Target = t;
	public bool CheckAmmo() => InMag > 0;

	public bool Reload()
	{
		if (Reloading || InMag >= MagSize || Reserve <= 0) return false;
		Reloading = true;
		_reloadLeft = ReloadFor(ReloadTime, 1f, 1f);
		return true;
	}

	public bool PrimaryFire(Vector3 aimPoint) => Fire(aimPoint, 1f);

	public bool SecondaryFire(Vector3 aimPoint) => Fire(aimPoint, 1.15f);

	bool Fire(Vector3 aimPoint, float kineticMul)
	{
		if (Reloading || InMag <= 0) return false;
		var from = GlobalPosition + Vector3.Up * 1.4f;
		var dir = aimPoint - from;
		if (dir.LengthSquared() < 0.01f) dir = -GlobalTransform.Basis.Z;
		dir = dir.Normalized();
		InMag--;
		var packet = DamagePacket.KineticHit(Kinetic * kineticMul, Ap + Kit.ApAdd);
		packet.Name = "test-rifle";
		packet.Instigator = _owner;
		var exclude = _owner is CollisionObject3D body ? body.GetRid() : default;
		DamageCast.Ray(this, from, dir, Range * Kit.Vel, packet, exclude);
		return true;
	}
}

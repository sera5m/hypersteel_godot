using Godot;
using Hypersteel.Entity;
using Hypersteel.Health;

namespace Hypersteel.Damage;

/// <summary>Temp test: right click fires a damage-layer ray and prints soak.</summary>
public partial class DebugDamageRay : Node
{
	[Export] public bool enabled = true;
	[Export] public float range = 200f;
	[Export] public float kinetic = 45f;
	[Export] public int ap = 4;

	Camera3D _cam;
	Node3D _shooter;

	public override void _Ready()
	{
		_shooter = GetParent() as Node3D ?? GetOwner() as Node3D;
		_cam = _shooter?.GetNodeOrNull<Camera3D>("%Camera3D");
		_cam ??= _shooter?.FindChild("Camera3D", true, false) as Camera3D;
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (!enabled) return;
		if (e is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Right)
			Fire();
	}

	void Fire()
	{
		if (_cam == null)
		{
			GD.Print("[dbg-ray] no camera");
			return;
		}

		var packet = DamagePacket.KineticHit(kinetic, ap, DamageSource.Beam);
		packet.Name = "debug-ray";
		packet.KineticFlags = KineticFlags.Piercing;
		packet.Instigator = _shooter;

		Vector3 from = _cam.GlobalPosition;
		Vector3 dir = -_cam.GlobalTransform.Basis.Z;
		Rid skip = _shooter is CollisionObject3D co ? co.GetRid() : default;
		DamageCast.Ray(this, from, dir, range, packet, skip);
		GD.Print($"[dbg-ray] fired from {from} dir {dir}");
	}
}

using Godot;

namespace Hypersteel.Damage;

public partial class ProjectileMove : Node
{
	[Export] public ProjectileDef def;
	[Export] public bool debugTrace;

	public Vector3 Velocity;
	public Node3D Body;

	public override void _PhysicsProcess(double delta)
	{
		if (Body == null || def == null) return;
		float dt = (float)delta;
		if (def.gravity != 0f)
			Velocity += Vector3.Down * def.gravity * dt;
		if (def.drag > 0f)
			Velocity *= Math.Max(0f, 1f - def.drag * dt);
		Vector3 from = Body.GlobalPosition;
		Body.GlobalPosition = from + Velocity * dt;
		if (Velocity.LengthSquared() > 0.0001f)
			Body.LookAt(Body.GlobalPosition + Velocity, Vector3.Up);
		if (debugTrace || def.debugTrace)
			DebugDraw(from, Body.GlobalPosition);
	}

	static void DebugDraw(Vector3 a, Vector3 b)
	{
		DebugDraw3D? unused = null;
		// No addon required. MeshInstance debug is optional later.
		GD.Print($"[proj] {a} -> {b}");
	}
}

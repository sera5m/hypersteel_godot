using Godot;
using Hypersteel.Damage;
using Hypersteel.Health;

namespace Hypersteel.Entity;

/// <summary>Capsule person so we can test casts without a finished mesh.</summary>
public static class HumanoidStandIn
{
	public static void Attach(Node3D host)
	{
		if (host == null) return;
		if (host.GetNodeOrNull("StandIn") != null) return;

		var root = new Node3D { Name = "StandIn" };
		host.AddChild(root);

		AddPart(root, host, "Head", new Vector3(0f, 1.62f, 0f), 0.12f, 0.18f, BodySegment.Head);
		AddPart(root, host, "Torso", new Vector3(0f, 1.15f, 0f), 0.18f, 0.55f, BodySegment.Torso);
		AddPart(root, host, "ArmL", new Vector3(-0.32f, 1.2f, 0f), 0.07f, 0.5f, BodySegment.Arm);
		AddPart(root, host, "ArmR", new Vector3(0.32f, 1.2f, 0f), 0.07f, 0.5f, BodySegment.Arm);
		AddPart(root, host, "LegL", new Vector3(-0.12f, 0.45f, 0f), 0.09f, 0.7f, BodySegment.Leg);
		AddPart(root, host, "LegR", new Vector3(0.12f, 0.45f, 0f), 0.09f, 0.7f, BodySegment.Leg);
	}

	static void AddPart(Node3D root, Node3D host, string name, Vector3 pos, float radius, float height, BodySegment seg)
	{
		var area = new HurtBox
		{
			Name = name,
			segment = seg,
			bone = name,
			Position = pos,
		};
		var shape = new CollisionShape3D
		{
			Shape = new CapsuleShape3D { Radius = radius, Height = height },
		};
		var mesh = new MeshInstance3D
		{
			Mesh = new CapsuleMesh { Radius = radius, Height = height },
		};
		area.AddChild(shape);
		area.AddChild(mesh);
		root.AddChild(area);
	}
}

using Godot;

namespace Hypersteel.Health;

public readonly struct Hit
{
	public readonly float Amount;
	public readonly DamageKind Kind;
	public readonly BodySegment Segment;
	public readonly Vector3 Point;
public readonly Vector3 origin; //for hit traces, they might originate from enemy gun barrel
	public readonly Vector3 Normal;
	public readonly Node3D Instigator;
	public readonly StringName Bone;

	public Hit(
		float amount,
		DamageKind kind,
		BodySegment segment = BodySegment.Torso,
		Vector3 point = default,
		Vector3 normal = default,
		Node3D instigator = null,
		StringName bone = default)
	{
		Amount = amount;
		Kind = kind;
		Segment = segment;
		Point = point;
		Normal = normal;
		Instigator = instigator;
		Bone = bone;
	}

	public Hit WithSegment(BodySegment segment) =>
		new(Amount, Kind, segment, Point, Normal, Instigator, Bone);
}

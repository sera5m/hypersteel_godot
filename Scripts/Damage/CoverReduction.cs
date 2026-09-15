using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

/// <summary>One cover actor that cut this packet. Only allocated if the recipient asked.</summary>
public struct CoverReduction
{
	public Node3D Actor;
	public float Reduced;
	public float Through;
	public BodySegment Mount;
}

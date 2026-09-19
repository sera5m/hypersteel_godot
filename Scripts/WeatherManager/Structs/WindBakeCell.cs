using Godot;

namespace Hypersteel.Weather;

public struct WindBakeCell
{
	public Vector3 Direction;
	public WindTier Tier;
	public bool Occluded;
	public int Bounces;
	public int SegmentId;
}

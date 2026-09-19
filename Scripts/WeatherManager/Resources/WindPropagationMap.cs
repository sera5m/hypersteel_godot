using Godot;

namespace Hypersteel.Weather;

[GlobalClass]
public partial class WindPropagationMap : Resource
{
	[Export] public Vector3 origin;
	[Export] public Vector3 cellSize = new(4f, 4f, 4f);
	[Export] public Vector3I dim = new(32, 8, 32);
	[Export] public Vector3 prevailing;
	[Export] public WindTier sourceTier = WindTier.Wind;
	[Export] public Vector3[] dirs = System.Array.Empty<Vector3>();
	[Export] public int[] tiers = System.Array.Empty<int>();
	[Export] public byte[] occluded = System.Array.Empty<byte>();
	[Export] public int[] bounces = System.Array.Empty<int>();
	[Export] public int[] segmentIds = System.Array.Empty<int>();

	public int CellCount => dim.X * dim.Y * dim.Z;

	public bool TryIndex(Vector3 world, out int index)
	{
		index = -1;
		if (cellSize.X <= 0f) return false;
		var local = world - origin;
		int x = Mathf.FloorToInt(local.X / cellSize.X);
		int y = Mathf.FloorToInt(local.Y / cellSize.Y);
		int z = Mathf.FloorToInt(local.Z / cellSize.Z);
		if ((uint)x >= (uint)dim.X || (uint)y >= (uint)dim.Y || (uint)z >= (uint)dim.Z)
			return false;
		index = x + dim.X * (y + dim.Y * z);
		return index >= 0 && index < dirs.Length;
	}

	public WindSample Sample(Vector3 world)
	{
		if (!TryIndex(world, out var i))
			return WindSample.Still;
		return new WindSample
		{
			Direction = dirs[i],
			Tier = (WindTier)tiers[i],
			Occluded = occluded[i] != 0,
			FromBake = true,
		};
	}
}

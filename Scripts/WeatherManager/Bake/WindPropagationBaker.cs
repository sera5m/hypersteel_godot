using Godot;

namespace Hypersteel.Weather;

public sealed class WindPropagationBaker
{
	public int MaxBounces = 4;
	public uint CollisionMask = 1;

	public WindPropagationMap Bake(World3D world, Vector3 origin, Vector3I dim, Vector3 cellSize, Vector3 prevailing, WindTier sourceTier)
	{
		var map = new WindPropagationMap
		{
			origin = origin,
			dim = dim,
			cellSize = cellSize,
			prevailing = prevailing.Normalized(),
			sourceTier = sourceTier,
		};
		int count = dim.X * dim.Y * dim.Z;
		map.dirs = new Vector3[count];
		map.tiers = new int[count];
		map.occluded = new byte[count];
		map.bounces = new int[count];
		map.segmentIds = new int[count];
		var space = world.DirectSpaceState;
		var dir0 = map.prevailing;
		if (dir0.LengthSquared() < 0.0001f) dir0 = Vector3.Forward;
		for (int z = 0; z < dim.Z; z++)
		for (int y = 0; y < dim.Y; y++)
		for (int x = 0; x < dim.X; x++)
		{
			int i = x + dim.X * (y + dim.Y * z);
			var cellCenter = origin + new Vector3((x + 0.5f) * cellSize.X, (y + 0.5f) * cellSize.Y, (z + 0.5f) * cellSize.Z);
			TraceCell(space, cellCenter, dir0, sourceTier, out var sample, out var bounces);
			map.dirs[i] = sample.Direction;
			map.tiers[i] = (int)sample.Tier;
			map.occluded[i] = (byte)(sample.Occluded ? 1 : 0);
			map.bounces[i] = bounces;
			map.segmentIds[i] = i;
		}
		return map;
	}

	void TraceCell(PhysicsDirectSpaceState3D space, Vector3 start, Vector3 dir, WindTier tier, out WindSample sample, out int bounces)
	{
		sample = new WindSample { Direction = dir, Tier = tier, Occluded = false, FromBake = true };
		bounces = 0;
		var from = start - dir * 8f;
		var to = start;
		for (int n = 0; n <= MaxBounces && tier > WindTier.Still; n++)
		{
			var q = PhysicsRayQueryParameters3D.Create(from, to);
			q.CollisionMask = CollisionMask;
			var hit = space.IntersectRay(q);
			if (hit.Count == 0)
			{
				sample.Direction = dir;
				sample.Tier = tier;
				bounces = n;
				return;
			}
			var normal = (Vector3)hit["normal"];
			dir = dir.Bounce(normal).Normalized();
			tier = tier - 1;
			from = (Vector3)hit["position"] + normal * 0.05f;
			to = from + dir * 8f;
			bounces = n + 1;
		}
		sample.Direction = dir;
		sample.Tier = tier;
		sample.Occluded = tier <= WindTier.Still;
	}
}

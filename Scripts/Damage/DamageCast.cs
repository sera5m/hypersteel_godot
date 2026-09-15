using System.Collections.Generic;
using Godot;
using Hypersteel.Entity;

namespace Hypersteel.Damage;

public struct CastHit
{
	public Node Collider;
	public Vector3 Point;
	public Vector3 Normal;
	public float Distance;
	public Rid Rid;
}

public static class DamageCast
{
	public const int MaxBounces = 0;
	public const int MaxExits = 4;
	public const float ExitPad = 0.12f;

	public static void Ray(Node worldContext, Vector3 from, Vector3 dir, float length, DamagePacket packet, Rid exclude = default)
	{
		Tube(worldContext, from, dir, length, packet, 0f, exclude);
	}

	public static void Tube(Node worldContext, Vector3 from, Vector3 dir, float length, DamagePacket packet, float radius, Rid exclude = default)
	{
		PhysicsDirectSpaceState3D space = worldContext.GetViewport()?.World3D?.DirectSpaceState;
		if (space == null || length <= 0.001f) return;

		dir = dir.LengthSquared() > 0.0001f ? dir.Normalized() : Vector3.Forward;
		var excludeSet = new HashSet<Rid>();
		if (exclude.IsValid) excludeSet.Add(exclude);

		Vector3 origin = from;
		float left = length;
		DamagePacket shot = packet;
		int exits = 0;

		while (left > 0.01f && shot.Kinetic > 0.01f && exits <= MaxExits)
		{
			if (!TryFirst(space, origin, dir, left, radius, excludeSet, out CastHit hit))
				return;

			shot.Point = hit.Point;
			shot.Normal = hit.Normal;

			IDamageable target = DamageProbe.FindDamageable(hit.Collider);
			if (target == null)
				return;

			float before = shot.Kinetic;
			target.Hurt(shot);

			bool overpen = false;
			float through = 0f;
			if (target is ArmorPiece plate)
			{
				through = plate.LastThrough;
				overpen = plate.LastOverpen && through > 0.01f;
			}
			else if (target is ActorEntity)
			{
				overpen = (shot.KineticFlags & KineticFlags.Piercing) != 0 && before > 8f;
				through = before * 0.35f;
			}

			if (hit.Rid.IsValid) excludeSet.Add(hit.Rid);
			ExcludeTree(DamageProbe.FindEntity(hit.Collider), excludeSet);

			if (!overpen) return;

			Vector3 throughDir = dir.Normalized();
			Vector3 exit = ExitPoint(hit.Point, throughDir, hit.Collider);
			shot.Kinetic = through;
			shot.HasShrapnel = false;
			origin = exit;
			dir = throughDir;
			left = Math.Max(0f, left - hit.Distance - ExitPad);
			exits++;

			if (packet.HasShrapnel && packet.Shrapnel.Fragments > 0 && packet.Shrapnel.SpawnWorldIfExit)
				SpawnShrapnel(worldContext, exit, throughDir, shot, packet.Shrapnel, excludeSet);
		}
	}

	static bool TryFirst(PhysicsDirectSpaceState3D space, Vector3 from, Vector3 dir, float length, float radius, HashSet<Rid> exclude, out CastHit hit)
	{
		hit = default;
		Vector3 to = from + dir * length;

		var q = PhysicsRayQueryParameters3D.Create(from, to);
		q.CollisionMask = CollisionLayers.Trace;
		q.CollideWithAreas = true;
		q.CollideWithBodies = true;
		q.Exclude = ToGodotExclude(exclude);
		var raw = space.IntersectRay(q);
		if (raw != null && raw.Count > 0)
		{
			Fill(ref hit, raw, from);
			return hit.Collider != null;
		}

		if (radius <= 0.001f) return false;

		var capsule = new CapsuleShape3D { Radius = radius, Height = Math.Max(radius * 2f + 0.01f, 0.05f) };
		var sq = new PhysicsShapeQueryParameters3D
		{
			Shape = capsule,
			CollisionMask = CollisionLayers.Trace,
			CollideWithAreas = true,
			CollideWithBodies = true,
			Exclude = ToGodotExclude(exclude),
			Motion = to - from,
			Transform = CapsuleAt(from, dir),
		};
		var rest = space.GetRestInfo(sq);
		if (rest == null || rest.Count == 0) return false;
		Fill(ref hit, rest, from);
		return hit.Collider != null;
	}

	static void Fill(ref CastHit hit, Godot.Collections.Dictionary raw, Vector3 from)
	{
		hit.Collider = raw.ContainsKey("collider") ? raw["collider"].AsGodotObject() as Node : null;
		hit.Point = raw.ContainsKey("position") ? raw["position"].AsVector3() : from;
		hit.Normal = raw.ContainsKey("normal") ? raw["normal"].AsVector3() : Vector3.Up;
		hit.Rid = raw.ContainsKey("rid") ? raw["rid"].AsRid() : default;
		hit.Distance = hit.Point.DistanceTo(from);
	}

	static Transform3D CapsuleAt(Vector3 from, Vector3 dir)
	{
		Vector3 y = dir.Normalized();
		Vector3 x = y.Cross(Vector3.Up);
		if (x.LengthSquared() < 0.001f) x = y.Cross(Vector3.Right);
		x = x.Normalized();
		return new Transform3D(new Basis(x, y, x.Cross(y)), from);
	}

	static Vector3 ExitPoint(Vector3 entry, Vector3 dir, Node collider)
	{
		float extra = collider is CollisionObject3D ? Math.Max(ExitPad, 0.35f) : ExitPad;
		return entry + dir.Normalized() * extra;
	}

	static void ExcludeTree(Node n, HashSet<Rid> set)
	{
		if (n == null) return;
		if (n is CollisionObject3D co) set.Add(co.GetRid());
		foreach (Node child in n.GetChildren())
			ExcludeTree(child, set);
	}

	static Godot.Collections.Array<Rid> ToGodotExclude(HashSet<Rid> set)
	{
		var arr = new Godot.Collections.Array<Rid>();
		foreach (Rid r in set) arr.Add(r);
		return arr;
	}

	static void SpawnShrapnel(Node ctx, Vector3 origin, Vector3 dir, DamagePacket parent, ShrapnelRecipe recipe, HashSet<Rid> exclude)
	{
		int n = Math.Max(1, recipe.Fragments);
		float each = parent.Kinetic / n * (1f / n);
		Rid skip = default;
		foreach (Rid r in exclude) { skip = r; break; }
		for (int i = 0; i < n; i++)
		{
			Vector3 spray = (dir + new Vector3((i % 3) - 1, ((i / 3) % 3) - 1, 0.2f) * 0.15f).Normalized();
			var frag = parent;
			frag.Kinetic = each;
			frag.HasShrapnel = false;
			frag.Name = "shrapnel";
			if (recipe.Piercing) frag.KineticFlags |= KineticFlags.Piercing;
			Ray(ctx, origin, spray, 6f, frag, skip);
		}
	}
}

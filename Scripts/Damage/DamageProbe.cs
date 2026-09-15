using Godot;
using Hypersteel.Entity;

namespace Hypersteel.Damage;

public static class DamageProbe
{
	/// <summary>Walk parents until ActorEntity. Any child collider is damageable.</summary>
	public static ActorEntity FindEntity(Node from)
	{
		Node n = from;
		while (n != null)
		{
			if (n is ActorEntity actor)
				return actor;
			n = n.GetParent();
		}
		return null;
	}

	public static bool TryHurt(Node from, DamagePacket packet)
	{
		ActorEntity actor = FindEntity(from);
		if (actor == null) return false;
		actor.Hurt(packet);
		return true;
	}
}

using Godot;
using Hypersteel.Entity;

namespace Hypersteel.Damage;

public static class DamageProbe
{
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

	/// <summary>Nearest IDamageable walking up. ArmorPiece / CritSocket beat the wearer.</summary>
	public static IDamageable FindDamageable(Node from)
	{
		Node n = from;
		while (n != null)
		{
			if (n is IDamageable d)
				return d;
			n = n.GetParent();
		}
		return null;
	}

	public static bool TryHurt(Node from, DamagePacket packet)
	{
		IDamageable target = FindDamageable(from);
		if (target == null) return false;
		target.Hurt(packet);
		return true;
	}
}

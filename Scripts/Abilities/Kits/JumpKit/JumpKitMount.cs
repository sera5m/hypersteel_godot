using Godot;
using Hypersteel.Entity;

namespace Hypersteel.Abilities.Kits;

/// <summary>Compose helper. KitComposer calls this; no reflection.</summary>
public static class JumpKitMount
{
	public const string NodeName = "JumpKit";

	public static JumpKit Attach(Node host, JumpKitDef def)
	{
		if (host == null) return null;

		var kit = host.GetNodeOrNull<JumpKit>(NodeName);
		if (kit == null)
		{
			foreach (Node child in host.GetChildren())
			{
				if (child is JumpKit existing)
				{
					kit = existing;
					break;
				}
			}
		}

		if (kit == null)
		{
			kit = new JumpKit { Name = NodeName, def = def ?? new JumpKitDef() };
			host.AddChild(kit);
		}
		else if (kit.def == null)
		{
			kit.def = def ?? new JumpKitDef();
		}

		if (kit.Motor == null)
		{
			if (host is CharacterBody3D body)
				kit.BindBody(body);
		}

		if (host is ActorEntity actor)
			actor.jumpKit = kit;

		return kit;
	}
}

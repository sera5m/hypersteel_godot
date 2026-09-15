using Godot;
using Hypersteel.Health;

namespace Hypersteel.Damage;

public static class KitComposer
{
	public static void Compose(Node host, KitSheet kit)
	{
		if (host == null || kit == null) return;

		if (kit.hasHealth && host.GetNodeOrNull<HealthComponent>("Health") == null
		    && Find<HealthComponent>(host) == null)
		{
			var health = new HealthComponent { Name = "Health", rules = kit.healthRules ?? new HealthRuleConfs() };
			host.AddChild(health);
		}

		if (kit.hasStatus && Find<StatusComponent>(host) == null)
		{
			var status = new StatusComponent { Name = "Status", rules = kit.status ?? new StatusRules() };
			host.AddChild(status);
		}

		if (kit.hasFeelings && Find<FeelingsComponent>(host) == null)
		{
			var feelings = new FeelingsComponent { Name = "Feelings", rules = kit.feelings ?? new FeelingsRules() };
			host.AddChild(feelings);
		}
	}

	static T Find<T>(Node host) where T : Node
	{
		foreach (Node child in host.GetChildren())
		{
			if (child is T t) return t;
		}
		return host.FindChild(typeof(T).Name.Replace("Component", ""), true, false) as T;
	}
}

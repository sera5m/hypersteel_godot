using Godot;
using Hypersteel.Abilities.Kits;
using Hypersteel.Weapons;

namespace Hypersteel.Entity;

/// <summary>
/// One verb board. Player input and NpcBrain both call this.
/// Keys live in OperatorInput. AI never reads Input.
/// </summary>
public partial class PawnActions : Node
{
	[Export] public bool AdsToggle;

	public ActorEntity Body { get; private set; }
	public Gun Gun { get; private set; }
	public bool Ads { get; private set; }
	public bool Slinged { get; private set; }

	public void Bind(ActorEntity body)
	{
		Body = body;
		Gun = body?.FindChild("TestRifle", true, false) as Gun
		      ?? body?.FindChild("Primary", true, false) as Gun;
	}

	public void SetGun(Gun gun) => Gun = gun;

	public bool Fire(Vector3 aim)
	{
		if (Slinged || Gun is not TestRifle rifle) return false;
		return rifle.PrimaryFire(aim);
	}

	public bool Alt(Vector3 aim)
	{
		if (Slinged || Gun is not TestRifle rifle) return false;
		return rifle.SecondaryFire(aim);
	}

	public void SetAds(bool on)
	{
		Ads = on;
		Gun?.SetAds(on);
	}

	public void AdsButton(bool pressed, bool justPressed)
	{
		if (AdsToggle)
		{
			if (justPressed) SetAds(!Ads);
			return;
		}
		SetAds(pressed);
	}

	public bool Reload() => Gun is TestRifle r && r.Reload();

	public bool Mode() => Gun != null && Gun.CycleMode();

	public void Inspect(bool on) => Gun?.SetInspect(on);

	public bool KitPulse(Vector3 wish)
	{
		if (Body?.jumpKit == null) return false;
		return Body.jumpKit.TryPulse(JumpVerb.Dash, wish);
	}

	public void Sling(bool away)
	{
		Slinged = away;
		if (away) SetAds(false);
	}
}

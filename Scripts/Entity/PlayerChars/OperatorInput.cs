using Godot;
using Hypersteel.Weapons;

namespace Hypersteel.Entity.PlayerChars;

/// <summary>
/// Reads the 24 map. Calls PawnActions. Not a second gun.
/// Action names match 24-player-controls.md. Add them in Project Settings if missing.
/// </summary>
public partial class OperatorInput : Node
{
	[Export] public bool AdsToggle;
	[Export] public NodePath bodyPath;

	PawnActions _acts;
	ActorEntity _body;

	public override void _Ready()
	{
		_body = GetNodeOrNull<ActorEntity>(bodyPath);
		_body ??= GetParent() as ActorEntity;
		_acts = GetParent()?.GetNodeOrNull<PawnActions>("PawnActions");
		if (_acts == null && _body != null)
		{
			_acts = new PawnActions { Name = "PawnActions", AdsToggle = AdsToggle };
			_body.AddChild(_acts);
		}
		_acts?.Bind(_body);
		if (_acts != null) _acts.AdsToggle = AdsToggle;
	}

	public override void _UnhandledInput(InputEvent e)
	{
		if (_acts == null) return;

		if (e.IsActionPressed("ads")) _acts.AdsButton(true, true);
		if (e.IsActionReleased("ads")) _acts.AdsButton(false, false);

		if (e.IsActionPressed("fire"))
		{
			var aim = AimPoint();
			_acts.Fire(aim);
		}
		if (e.IsActionPressed("alt_fire")) _acts.Alt(AimPoint());
		if (e.IsActionPressed("reload")) _acts.Reload();
		if (e.IsActionPressed("interact")) _acts.Mode();
		if (e.IsActionPressed("jumpkit"))
		{
			var wish = _body != null ? -_body.GlobalTransform.Basis.Z : Vector3.Forward;
			_acts.KitPulse(wish);
		}
		if (e.IsActionPressed("sling")) _acts.Sling(!_acts.Slinged);
		if (e.IsActionPressed("inspect")) _acts.Inspect(true);
	}

	Vector3 AimPoint()
	{
		if (_body == null) return Vector3.Zero;
		return _body.GlobalPosition - _body.GlobalTransform.Basis.Z * 40f + Vector3.Up * 1.4f;
	}
}

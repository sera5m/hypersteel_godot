using Godot;

namespace Hypersteel.Abilities.Kits;

/// <summary>
/// Mounted appliance. Bar + cooldown + pulse/hold triggers.
/// Owner maps a motor in BindMotor. Controllers watch and call TryPulse / SetHold.
/// </summary>
[GlobalClass]
public partial class JumpKit : Node
{
	[Export] public JumpKitDef def;

	public IMoveMotor Motor { get; private set; }
	public CharacterBodyMoveMotor BodyMotor { get; private set; }

	public float Current { get; private set; }
	public float Max => def != null ? def.capacity : 0f;
	public float BusyUntil { get; private set; }
	public bool IsBusy => TimeLeftBusy() > 0f;
	public bool IsHovering { get; private set; }
	public bool IsFlying { get; private set; }
	public JumpVerb LastVerb { get; private set; }

	Vector3 _preDashVel;
	Vector3 _dashDir;
	bool _dashLock;
	bool _wallrunning;

	[Signal] public delegate void ChangedEventHandler(float current, float max);
	[Signal] public delegate void FiredEventHandler(int verb);

	public override void _Ready()
	{
		if (def == null) def = new JumpKitDef();
		Current = Mathf.Clamp(def.startFilled, 0f, def.capacity);
		EmitSignal(SignalName.Changed, Current, Max);
	}

	public void BindMotor(IMoveMotor motor)
	{
		Motor = motor;
		BodyMotor = motor as CharacterBodyMoveMotor;
	}

	public void BindBody(CharacterBody3D body)
	{
		var wrap = new CharacterBodyMoveMotor(body);
		BindMotor(wrap);
	}

	public bool Has(JumpVerb verb) => def != null && verb switch
	{
		JumpVerb.AirJump => def.hasAirJump,
		JumpVerb.Dash => def.hasDash,
		JumpVerb.Hover => def.hasHover,
		JumpVerb.Slam => def.hasSlam,
		JumpVerb.Flight => def.hasFlight,
		_ => false,
	};

	public JumpTrigger TriggerOf(JumpVerb verb) =>
		verb is JumpVerb.Hover or JumpVerb.Flight ? JumpTrigger.Hold : JumpTrigger.Pulse;

	public float CostOf(JumpVerb verb)
	{
		if (def == null) return float.MaxValue;
		return verb switch
		{
			JumpVerb.AirJump => def.airJumpCost,
			JumpVerb.Dash => def.dashCost,
			JumpVerb.Slam => def.slamDumpsBar ? Current : Current,
			JumpVerb.Hover => 0f,
			JumpVerb.Flight => 0f,
			_ => float.MaxValue,
		};
	}

	public bool CanPulse(JumpVerb verb)
	{
		if (Motor == null || def == null || !Has(verb)) return false;
		if (TriggerOf(verb) != JumpTrigger.Pulse) return false;
		if (IsBusy) return false;
		if (verb == JumpVerb.Slam) return Current > 0.05f;
		return Current + 0.001f >= CostOf(verb);
	}

	public bool CanHold(JumpVerb verb) =>
		Motor != null && def != null && Has(verb) && TriggerOf(verb) == JumpTrigger.Hold && Current > 0.01f;

	public bool TryPulse(JumpVerb verb, Vector3 wishDir)
	{
		if (!CanPulse(verb)) return false;
		if (verb == JumpVerb.Slam)
			Spend(Current);
		else if (!Spend(CostOf(verb)))
			return false;

		LastVerb = verb;
		switch (verb)
		{
			case JumpVerb.AirJump:
				PulseAirJump();
				break;
			case JumpVerb.Dash:
				PulseDash(wishDir);
				break;
			case JumpVerb.Slam:
				PulseSlam();
				break;
			default:
				return false;
		}

		EmitSignal(SignalName.Fired, (int)verb);
		EmitSignal(SignalName.Changed, Current, Max);
		return true;
	}

	public void SetHold(JumpVerb verb, bool held, Vector3 wishDir)
	{
		if (verb == JumpVerb.Hover)
		{
			IsHovering = held && CanHold(JumpVerb.Hover);
			if (Motor != null)
				Motor.GravityScale = IsHovering ? def.hoverGravityScale : (IsFlying ? def.flightGravityScale : 1f);
			return;
		}

		if (verb == JumpVerb.Flight)
		{
			IsFlying = held && CanHold(JumpVerb.Flight);
			if (Motor != null)
				Motor.GravityScale = IsFlying ? def.flightGravityScale : (IsHovering ? def.hoverGravityScale : 1f);
			if (IsFlying && wishDir.LengthSquared() > 0.01f)
				Motor.ApplyImpulse(wishDir.Normalized() * def.dashSpeed * 0.02f);
		}
	}

	public bool Spend(float amount)
	{
		if (amount <= 0f) return true;
		if (Current < amount) return false;
		Current -= amount;
		EmitSignal(SignalName.Changed, Current, Max);
		return true;
	}

	public void AddFuel(float amount)
	{
		if (def == null || amount == 0f) return;
		Current = Mathf.Clamp(Current + amount, 0f, def.capacity);
		EmitSignal(SignalName.Changed, Current, Max);
	}

	public void SetCurrent(float value)
	{
		if (def == null) return;
		Current = Mathf.Clamp(value, 0f, def.capacity);
		EmitSignal(SignalName.Changed, Current, Max);
	}

	public void Refill() => SetCurrent(Max);

	public void NotifyLanded()
	{
		if (def == null) return;
		if (Current < def.landRefundIfBelow)
			AddFuel(def.landRefundAmount);
	}

	public void NotifyHooked()
	{
		if (def != null && def.rechargeOnHook)
			Refill();
	}

	public void NotifyKilled() => Refill();

	public void SetWallrunning(bool on) => _wallrunning = on;

	public float TimeUntilReady() => TimeLeftBusy();

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;
		if (def == null || Motor == null) return;

		BodyMotor?.TickOverride(dt);

		if (_dashLock && (BodyMotor == null || !BodyMotor.OverrideActive))
			EndDash();

		float drain = 0f;
		if (IsHovering) drain += def.hoverDrainPerSec;
		if (IsFlying) drain += def.flightDrainPerSec;
		if (drain > 0f)
		{
			Current = Mathf.Max(0f, Current - drain * dt);
			if (Current <= 0.01f)
			{
				IsHovering = false;
				IsFlying = false;
				Motor.GravityScale = 1f;
			}
			EmitSignal(SignalName.Changed, Current, Max);
		}
		else if (!IsBusy)
		{
			float regen = _wallrunning ? def.wallrunRegenPerSec : def.passiveRegenPerSec;
			if (regen > 0f && Current < def.capacity)
				AddFuel(regen * dt);
		}
	}

	void PulseAirJump()
	{
		Vector3 v = Motor.Velocity;
		v.Y = Mathf.Max(v.Y, def.airJumpSpeed);
		Motor.Velocity = v;
	}

	void PulseDash(Vector3 wishDir)
	{
		Vector3 dir = wishDir.LengthSquared() > 0.0001f ? wishDir.Normalized() : Motor.Forward;
		dir.Y = 0f;
		if (dir.LengthSquared() < 0.0001f) dir = Motor.Forward;
		dir = dir.Normalized();

		_preDashVel = Motor.Velocity;
		_dashDir = dir;
		_dashLock = true;
		BusyUntil = TimeNow() + def.dashSeconds;
		Motor.OverrideVelocity(dir * def.dashSpeed, def.dashSeconds);
	}

	void EndDash()
	{
		if (!_dashLock) return;
		_dashLock = false;
		BusyUntil = 0f;
		Motor.RestoreVelocity(_preDashVel + _dashDir);
	}

	void PulseSlam()
	{
		Vector3 dir = Motor.Forward.Normalized();
		if (dir.LengthSquared() < 0.0001f) dir = Vector3.Down;
		Motor.ApplyImpulse(dir * def.slamSpeed);
		BusyUntil = TimeNow() + 0.15f;
	}

	float TimeNow() => (float)Time.GetTicksMsec() / 1000f;

	float TimeLeftBusy()
	{
		float left = BusyUntil - TimeNow();
		return left > 0f ? left : 0f;
	}
}

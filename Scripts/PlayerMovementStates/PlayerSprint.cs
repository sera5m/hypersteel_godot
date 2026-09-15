using Godot;
using System;

[GlobalClass, Icon("res://addons/finite_state_machine/state_icon.png")]
public partial class PlayerSprint : PlayerMovementState
{
    public override void Enter()
    {
        base.Enter();
    }

    public override void PhysicsUpdate(double delta)
    {
        Movement.Stand(delta);

        Movement.currentSpeed = Mathf.Lerp(Movement.currentSpeed, Movement.sprintingSpeed,
                            1.0f - Mathf.Pow(0.5f, (float)delta * Movement.lerpSpeed));

        if (Input.IsActionPressed("crouch") && !Movement.IsOnWall() && !Movement.IsRunningUpSlope() && !Movement.stepCast.IsColliding()
                && Movement.Velocity.Length() >= (Movement.sprintingSpeed - 1) && Movement.inputDirection.Y < 0f)
        {
            EmitSignal(SignalName.StateFinished, "PlayerSlide", new());
        }

        if (!Movement.sprintAction)
        {
            EmitSignal(SignalName.StateFinished, "PlayerWalk", new());
        }

        if (Movement.inputDirection == Vector2.Zero)
            EmitSignal(SignalName.StateFinished, "PlayerIdle", new());

        if (Movement.CheckVault(delta, out Vector3 vaultPoint) && Input.IsActionJustPressed("jump"))
        {
            EmitSignal(SignalName.StateFinished, "PlayerVault", new());
        }

        if (!Movement.OnGround && Mathf.Abs(Movement.Velocity.Y) > 0.1f)
		{
			EmitSignal(SignalName.StateFinished, "PlayerAir", new());
		}

        if (Movement.CheckLadder())
        {
            EmitSignal(SignalName.StateFinished, "PlayerLadder", new());
        }
    }
}

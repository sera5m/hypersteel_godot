using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using MEC;

[GlobalClass, Icon("res://addons/finite_state_machine/state_icon.png")]
public partial class PlayerWallrun : PlayerMovementState
{
    public static Action WallRunStart;
    public static Action<Node> WallRunEnd;

    private float _frictionForce;
    private KinematicCollision3D _collision;
    public String wallDirection;
    
    public override void Enter()
    {
        base.Enter();
        WallRunStart?.Invoke();
        _frictionForce = Movement.wallFrictionCoefficient * Movement.gravity; 
        Movement.momentum = Movement.Velocity.Length();
        Movement.camState = CameraState.Wallrunning;
        Movement.CheckWall(out _collision, out wallDirection);
    }

    public override void Exit()
    {
        WallRunEnd?.Invoke(Movement);
        Movement.camState = CameraState.Normal;
    }

    public override void HandleInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion eventMouseMotion)
        {
            float rotationDegLeft = wallDirection == "Left" ? -60f : 0f;
            float rotationDegRight = wallDirection == "Right" ? 60f : 0f;
            Movement.RotatePlayerByConstraint(eventMouseMotion.Relative.X, eventMouseMotion.Relative.Y, rotationDegLeft, rotationDegRight);
        }
    }

    public override void PhysicsUpdate(double delta)
    {
        Vector3 rayDirection = Movement.GlobalBasis.X;
        if (wallDirection == "Left")
            rayDirection = -Movement.GlobalBasis.X; 

        bool wall = Movement.SendRayInDirection(rayDirection, 1f, out Vector3 wallNormal, out Vector3 wallPoint);

        if (wall && Movement.wallRunTimer <= Movement.wallRunTime)
        {
            Movement.wallRunTimer += (float)delta;
            float normalizedTime = Movement.wallRunTimer / Movement.wallRunTime;
            Movement.currentSpeed = Mathf.Lerp(Movement.currentSpeed, Movement.wallRunSpeed, normalizedTime);
            Vector3 rotatedVector = wallNormal.Rotated(Vector3.Up, Mathf.DegToRad(wallDirection == "Right" ? -90f : 90f));
            Movement.playerVelocity.Y -= _frictionForce * (float)delta;
            _frictionForce = Mathf.Lerp(_frictionForce, Movement.gravity, normalizedTime / 4);
            Movement.direction = Movement.direction.Lerp(rotatedVector.Normalized(), 
                1.0f - Mathf.Pow(0.5f, (float)delta * Movement.lerpSpeed));

            if (Input.IsActionJustPressed("jump"))
            {
                Movement.WallJump(wallNormal);
                EmitSignal(SignalName.StateEntered, "PlayerAir", new());
            }
        }
        else
        {
            EmitSignal(SignalName.StateFinished, "PlayerAir", new());
        }
        
        if (Movement.OnGround 
        || (Movement.SendRayInDirection(-Movement.GlobalBasis.Z, 0.5f, out Vector3 normal, out Vector3 point)))
            EmitSignal(SignalName.StateFinished, "PlayerAir", new());

        if (Movement.CheckVault(delta, out Vector3 vaultPoint))
        {
            Movement.wallRunTimer = 0f;
            EmitSignal(SignalName.StateFinished, "PlayerVault", new());
        }

        Movement.Velocity = Movement.playerVelocity;
    }
}

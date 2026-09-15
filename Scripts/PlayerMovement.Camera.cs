public partial class PlayerMovement
{
    public override void _Input(InputEvent @event)
    {
		// Mouse movement on camera
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			_yawDelta += eventMouseMotion.Relative.X;

			switch (camState)
			{
				case CameraState.Freelooking:
					if (FSM.CurrentState is PlayerSlide)
					{
						RotatePlayer(eventMouseMotion.Relative.X, eventMouseMotion.Relative.Y);
					}
					else
					{
						FreeLookRotation(eventMouseMotion.Relative.X, -120f, 120f);
					}

					break;

				case CameraState.Wallrunning:
					break;
				
				case CameraState.Normal:
				
					RotatePlayer(eventMouseMotion.Relative.X, eventMouseMotion.Relative.Y);
					break;
			}
		}
    }

    public override void _Process(double delta)
    {
		HandleZRotation((float)delta);

		if (Input.IsKeyPressed(Key.R) && _resetPosition != null)
			GlobalPosition = _resetPosition.GlobalPosition;

		PhysicsInterpolation();

		HandleFOV(delta);

		HandleLabels();
    }

	private void RotatePlayer(float mouseX, float mouseY)
	{
		RotateY(Mathf.DegToRad(-mouseX * mouseSensitivityX));

		_rotationX += Mathf.DegToRad(-mouseY * mouseSensitivityY);
		_rotationX = Mathf.Clamp(_rotationX, Mathf.DegToRad(-89f), Mathf.DegToRad(89f));

		_rotationZ += Mathf.DegToRad(mouseX * mouseSensitivityX * inputDirection.Length());
		_rotationZ = Mathf.Clamp(_rotationZ, Mathf.DegToRad(-_zClamp), Mathf.DegToRad(_zClamp));
	}

	public void RotatePlayerByConstraint(float mouseX, float mouseY, float leftDeg, float rightDeg)
	{
		float rotationY = Mathf.DegToRad(-mouseX * mouseSensitivityX);
		rotationY = Mathf.Clamp(rotationY, Mathf.DegToRad(leftDeg), Mathf.DegToRad(rightDeg));

		RotateY(rotationY);

		_rotationX += Mathf.DegToRad(-mouseY * mouseSensitivityY);
		_rotationX = Mathf.Clamp(_rotationX, Mathf.DegToRad(-89f), Mathf.DegToRad(89f));

		_rotationZ += Mathf.DegToRad(mouseX * mouseSensitivityX * inputDirection.Length());
		_rotationZ = Mathf.Clamp(_rotationZ, Mathf.DegToRad(-_zClamp), Mathf.DegToRad(_zClamp));
	}

	public void RotatePlayerByConstraintUp(float mouseX, float mouseY, float leftDeg, float rightDeg, float upDeg, float downDeg)
	{
		float rotationY = Mathf.DegToRad(-mouseX * mouseSensitivityX);
		rotationY = Mathf.Clamp(rotationY, Mathf.DegToRad(leftDeg), Mathf.DegToRad(rightDeg));

		RotateY(rotationY);

		_rotationX += Mathf.DegToRad(-mouseY * mouseSensitivityY);
		_rotationX = Mathf.Clamp(_rotationX, Mathf.DegToRad(downDeg), Mathf.DegToRad(upDeg));

		_rotationZ += Mathf.DegToRad(mouseX * mouseSensitivityX * inputDirection.Length());
		_rotationZ = Mathf.Clamp(_rotationZ, Mathf.DegToRad(-_zClamp), Mathf.DegToRad(_zClamp));
	}

	private void FreeLookRotation(float mouseX, float leftDeg, float rightDeg)
	{
		_neck.RotateY(Mathf.DegToRad(-mouseX * mouseSensitivityX));

		float neckClampedRotation = Mathf.Clamp(_neck.Rotation.Y, Mathf.DegToRad(leftDeg), Mathf.DegToRad(rightDeg));
		Vector3 neckRotation = new Vector3(_neck.Rotation.X, neckClampedRotation, _neck.Rotation.Z);
		_neck.Rotation = neckRotation;
	}

	public void ConstraintedRotation(float mouseX, float mouseY, float leftDeg, float rightDeg, float upDeg, float downDeg)
	{
		_neck.RotateY(Mathf.DegToRad(-mouseX * mouseSensitivityX));

		float neckClampedRotationY = Mathf.Clamp(_neck.Rotation.Y, Mathf.DegToRad(leftDeg), Mathf.DegToRad(rightDeg));
		Vector3 neckRotation = new Vector3(_neck.Rotation.X, neckClampedRotationY, _neck.Rotation.Z);
		_neck.Rotation = neckRotation;

		_rotationX += Mathf.DegToRad(-mouseY * mouseSensitivityY);
		_rotationX = Mathf.Clamp(_rotationX, Mathf.DegToRad(downDeg), Mathf.DegToRad(upDeg));

		Vector3 eyeRot = new Vector3(_eyes.Rotation.X, _eyes.Rotation.Y, 0f);
		_eyes.Rotation = _eyes.Rotation.Lerp(eyeRot, 1.0f - Mathf.Pow(0.5f, (float)GetProcessDeltaTime() * lerpSpeed));
	}

	private void PhysicsInterpolation()
	{
		if (_physicsInterpolate)
		{
			double fraction = Engine.GetPhysicsInterpolationFraction();
		
			Transform3D modifiedTransform = _mesh.GlobalTransform;
			modifiedTransform.Origin = _lastPhysicsPos.Lerp(GlobalTransform.Origin, (float)fraction);

			_mesh.GlobalTransform = modifiedTransform;
		}
	}

	private void HandleFOV(double delta)
	{
		if (_camera != null)
		{
			Vector3 playerVelocity = Velocity;

			float velocityMagnitude = playerVelocity.Length() / _maxPlayerVelocity;
			float velocityScale = Mathf.Pow(velocityMagnitude, _velocityExponent);

			float desiredFOV = Mathf.Lerp(_minFov, _maxFov, velocityScale);

			desiredFOV = Mathf.Clamp(desiredFOV, _minFov, _maxFov);

			_camera.Fov = Mathf.Lerp(_camera.Fov, desiredFOV, _fovLerpSpeed * (float)delta);
		}

	}

	private void HandleZRotation(float delta)
	{
		_rotationZ = Mathf.Lerp(_rotationZ, 0f, delta * _zRotationLerp);

		Transform3D transform = _head.Transform;
		transform.Basis = Basis.Identity;
		_head.Transform = transform;

		_head.RotateObjectLocal(Vector3.Right, _rotationX);
		_head.RotateObjectLocal(Vector3.Forward, _rotationZ);
	}

	private void HandleLabels()
	{
		if (_speedLabel == null)
			return;

		_speedLabel.Text = $"VELOCITY: {Velocity.Length()}";
		_momentumLabel.Text = $"MOMENTUM: {Mathf.Round(momentum)}";
		_stateLabel.Text = $"STATE: {FSM.CurrentState.Name}";
		_desiredSpeedLabel.Text = $"DESIRED SPEED: {Mathf.Round(currentSpeed)}";
		_previousStateLabel.Text = $"PREVIOUS STATE: {FSM.PreviousState.Name}";
	}

	private void HandleAnimation()
	{
		if (_animator != null)
		{
			_animator.Set("parameters/moveState/conditions/idle", FSM.CurrentState is PlayerIdle 
						|| (inputDirection.Length() <= 0.1f 
						&& (FSM.CurrentState is not PlayerWallrun || FSM.CurrentState is not PlayerVerticalWallrun)));

			_animator.Set("parameters/moveState/conditions/moving", IsOnFloor() && (FSM.CurrentState is PlayerWalk 
						|| FSM.CurrentState is PlayerSprint || FSM.CurrentState is PlayerCrouch) && Velocity.Length() > 0.1f);
			
			_animator.Set("parameters/moveState/conditions/jump", (Input.IsActionJustPressed("jump") && airTime < _coyoteTime && 
						_jumpsDone < _jumps && !ceilingRay.IsColliding() && (!stepCast.IsColliding() 
						|| FSM.CurrentState is PlayerIdle)) || 
						(Input.IsActionJustPressed("jump") && (FSM.CurrentState is PlayerWallrun || FSM.CurrentState is PlayerLadder)));

			_animator.Set("parameters/moveState/conditions/inAir", FSM.CurrentState is PlayerAir);
			
			_animator.Set("parameters/moveState/conditions/vault", FSM.CurrentState is PlayerVault);

			_animator.Set("parameters/moveState/conditions/wallrun", FSM.CurrentState is PlayerWallrun 
						|| FSM.CurrentState is PlayerVerticalWallrun);

			_animator.Set("parameters/moveState/conditions/isLadder", FSM.CurrentState is PlayerLadder && inputDirection.Length() >= 0.9f);


			_animator.Set("parameters/moveState/wallrun/conditions/left_wallrun",
						_wallRunStateNode.wallDirection == "Right" ? true : false);

			_animator.Set("parameters/moveState/wallrun/conditions/right_wallrun",
						_wallRunStateNode.wallDirection == "Left" ? true : false);


			if (_animationLabel != null)
			{
				AnimationNodeStateMachinePlayback node = (AnimationNodeStateMachinePlayback)_animator.Get("parameters/moveState/playback");
				_animationLabel.Text = "ANIMATION: " + node.GetCurrentNode();
			}
		}
	}
}

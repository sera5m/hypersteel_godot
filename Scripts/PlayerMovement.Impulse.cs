public partial class PlayerMovement
{
	private void HandleJump(float jumpSpeed)
	{
		// Handle Jump (Must refactor)
		bool canCoyote = sauce == null || sauce.coyoteAffectsJump;
		if (Input.IsActionJustPressed("jump") && ((ActuallyGrounded && !ceilingRay.IsColliding()) || (InCoyote && canCoyote) || airTime < _coyoteTime)
			&& (!stepCast.IsColliding() || FSM.CurrentState is PlayerIdle))
		{
			// Jump
			if (_jumpsDone < _jumps)
			{
				Jump(jumpSpeed);
			}
		}
		else if (Input.IsActionJustPressed("jump") && _jumpsDone > 0 && _jumpsDone < _jumps)
		{
			// Jump
			Jump(jumpSpeed);
		}
	}

	public void Jump(float jumpSpeed)
	{
		Vector3 wish = SourceMove.WishDirection(Transform.Basis, inputDirection);
		if (sauce != null && sauce.lurchEnabled)
			SourceMove.LurchIntoWish(ref playerVelocity, wish, sauce.jumpLurch);

		if (FSM.CurrentState is PlayerVault)
		{
			Velocity = playerVelocity + jumpSpeed * Vector3.Up;
		}
		else
		{
			playerVelocity.Y = jumpSpeed;
		}
		
		_jumpsDone++;
	}

	public void ApplyFireRecoil(float strength)
	{
		if (sauce == null || !sauce.recoilEnabled) return;
		Vector3 back = GlobalBasis.Z; // Godot forward is -Z, so +Z is reverse of look
		playerVelocity += back * strength * sauce.fireRecoilScale;
		Velocity = playerVelocity;
	}

	public void ApplyPunchRecoil()
	{
		if (sauce == null || !sauce.recoilEnabled) return;
		playerVelocity += GlobalBasis.Z * sauce.punchRecoil;
		Velocity = playerVelocity;
	}

	public void ApplyShockwave(Vector3 fromWorld, float strength)
	{
		if (sauce == null || !sauce.recoilEnabled) return;
		Vector3 away = GlobalPosition - fromWorld;
		away.Y = Mathf.Max(away.Y, 0.15f);
		if (away.LengthSquared() < 0.0001f) away = Vector3.Up;
		playerVelocity += away.Normalized() * strength * sauce.shockwaveScale;
		Velocity = playerVelocity;
	}

	public void WallJump(Vector3 wallDir)
	{
		playerVelocity.Y = wallJumpSpeed / 2; 
		direction = ((wallDir.Normalized()) + (-_camera.GlobalBasis.Z.Normalized() / 4)) * (wallJumpSpeed / 16);
	}

	public void RollPlayer(Vector3 directionFromAir)
	{
		// Check if current direction from camera is close to the direction from air.
		
		// Check if there's some forward velocity

		// Then land and execute animation.
	}
}

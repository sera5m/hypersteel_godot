public partial class PlayerMovement
{
	public bool CheckVault(double delta, out Vector3 vaultPoint)
	{
		vaultPoint = default;
		Vector3 rawProjectedXZ = new Vector3(inputDirection.X, 0f, inputDirection.Y);
		_vaultProjection = _vaultProjection.Lerp(rawProjectedXZ, 25f * (float)delta);
		Vector3 inputProjectionPoint = 3f * _vaultProjection;
		_vaultRay.Position = new Vector3(inputProjectionPoint.X, _vaultRay.Position.Y, inputProjectionPoint.Z);
		float angleToFloor = Mathf.RadToDeg(GlobalBasis.Z.AngleTo(GetFloorNormal()));
		float vaultElevation = 0f;
		if (_vaultRay.IsColliding())
		{
			_vaultPoint = _vaultRay.GetCollisionPoint();
			vaultElevation = Math.Abs((_vaultPoint - GlobalPosition).Y);
			_vaultCast.Enabled = true;
		}
		else _vaultCast.Enabled = false;
		_vaultCast.GlobalPosition = _vaultPoint;
		if (inputDirection.Y < 0f && !ceilingRay.IsColliding() && _vaultCast.IsColliding() && _vaultCheck.IsColliding() && !stepCast.IsColliding() && (angleToFloor > 80f || Mathf.IsZeroApprox(angleToFloor)))
		{
			if (vaultElevation > 0.25f || IsOnWall())
			{
				vaultPoint = _vaultPoint;
				return true;
			}
		}
		return false;
	}

	public bool CheckLadder()
	{
		if (currentLadder == null || !isLadder) return false;
		Node3D area = currentLadder.GetNode<Node3D>("LadderArea/LadderCollider");
		if (area == null) return false;
		SendRayInDirection(-GlobalBasis.Z, 0.5f, out Vector3 rayNormal, out Vector3 rayPoint, out GodotObject collider);
		if (Mathf.Abs(rayNormal.Dot(Vector3.Up)) >= 0.3f) return false;
		float forwardAngle = Mathf.RadToDeg((-GlobalBasis.Z).AngleTo(-rayNormal));
		Vector3 ladderZOffset = -rayNormal * -0.5f;
		float nearestBarHeight = Mathf.Round(GlobalPosition.Y / currentLadder.BarSpacing) * currentLadder.BarSpacing + 1f;
		Vector3 wallPoint = new Vector3(area.GlobalPosition.X, nearestBarHeight, area.GlobalPosition.Z);
		if (forwardAngle > 0 && forwardAngle < 20f && inputDirection.Y < 0f)
		{
			GlobalPosition = wallPoint + ladderZOffset;
			return true;
		}
		return false;
	}

	private void ResetJumps() => _jumpsDone = 0;

	public bool IsRunningUpSlope() => GetFloorNormal().Dot(-Transform.Basis.Z) < 0f;

	public bool CheckWall(out KinematicCollision3D collision, out String direction)
	{
		direction = string.Empty;
		collision = default;
		int count = GetSlideCollisionCount();
		if (count <= 0 || inputDirection.Y >= 0) return false;
		for (int i = 0; i < count; i++)
		{
			KinematicCollision3D c = GetSlideCollision(i);
			Vector3 n = c.GetNormal();
			if (Mathf.Abs(n.Dot(Vector3.Up)) >= 0.1f) continue;
			float angleToWall = Mathf.RadToDeg(GlobalBasis.Z.AngleTo(n));
			float signedAngle = Mathf.RadToDeg(GlobalBasis.Z.SignedAngleTo(n, Vector3.Up));
			if (angleToWall < 105f && angleToWall > 25f)
			{
				direction = Mathf.Sign(signedAngle) > 0 ? "Left" : "Right";
				collision = c;
				return true;
			}
		}
		return false;
	}

	public bool CheckVerticalWall(out Vector3 wallDirection, out Vector3 wallPoint)
	{
		wallDirection = default;
		wallPoint = default;
		if (!SendRayInDirection(-GlobalBasis.Z, 0.5f, out Vector3 rayNormal, out Vector3 rayPoint)) return false;
		if (Mathf.Abs(rayNormal.Dot(Vector3.Up)) >= 0.3f) return false;
		float forwardAngle = Mathf.RadToDeg(GlobalBasis.Z.AngleTo(rayNormal));
		wallDirection = GlobalBasis.X.Cross(rayNormal).Normalized();
		wallDirection = new Vector3(Mathf.Abs(wallDirection.X), Mathf.Abs(wallDirection.Y), Mathf.Abs(wallDirection.Z));
		wallPoint = rayPoint;
		return forwardAngle > 0 && forwardAngle < 20f && rayNormal.Dot(_camera.GlobalBasis.Y) > 0.2f && Velocity.Y >= 0;
	}

	public bool SendRayInDirection(Vector3 direction, float range, out Vector3 rayNormal, out Vector3 rayPoint)
	{
		return SendRayInDirection(direction, range, out rayNormal, out rayPoint, out _);
	}

	public bool SendRayInDirection(Vector3 direction, float range, out Vector3 rayNormal, out Vector3 rayPoint, out GodotObject collider)
	{
		PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
		Vector3 rayOrigin = _camera.GlobalPosition;
		Vector3 rayEnd = rayOrigin + (direction * range);
		rayNormal = default;
		rayPoint = default;
		collider = default;
		PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd, (1u << 1) | (1u << 4));
		parameters.HitBackFaces = false;
		parameters.HitFromInside = false;
		parameters.CollideWithAreas = true;
		parameters.CollideWithBodies = true;
		var rayArray = spaceState.IntersectRay(parameters);
		if (!rayArray.ContainsKey("collider")) return false;
		rayArray.TryGetValue("normal", out Variant normal);
		rayArray.TryGetValue("position", out Variant position);
		rayArray.TryGetValue("collider", out Variant colliderVariant);
		rayNormal = normal.AsVector3();
		rayPoint = position.AsVector3();
		collider = colliderVariant.AsGodotObject();
		return true;
	}
}

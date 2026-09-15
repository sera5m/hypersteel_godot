using System.Collections.Generic;

public partial class PlayerMovement
{
	public bool CheckVault(double delta, out Vector3 vaultPoint)
	{
		vaultPoint = default(Vector3);

		// Get the raw input direction and smooth it out
		Vector3 rawProjectedXZ = new Vector3(inputDirection.X, 0f,inputDirection.Y);
		_vaultProjection = _vaultProjection.Lerp(rawProjectedXZ, 25f * (float)delta);

		// Get the projected point based on input
		Vector3 inputProjectionPoint = 3f * _vaultProjection;
		Vector3 vaultNormal = default(Vector3);

		float vaultElevation = 0f;
		float minElevation = 0.25f; // Value that decides how much elevation needed to vault

		_vaultRay.Position = new Vector3(inputProjectionPoint.X, _vaultRay.Position.Y, inputProjectionPoint.Z);

		Vector3 playerForward = this.GlobalBasis.Z;
		float angleToFloor = Mathf.RadToDeg(playerForward.AngleTo(GetFloorNormal()));

		if (_vaultRay.IsColliding())
		{
			_vaultPoint = _vaultRay.GetCollisionPoint();
			vaultNormal = _vaultRay.GetCollisionNormal();

			vaultElevation = Math.Abs((_vaultPoint - this.GlobalPosition).Y);

			_vaultCast.Enabled = true;
		}
		else
		{
			_vaultCast.Enabled = false;
		}

		_vaultCast.GlobalPosition = _vaultPoint;

		if (inputDirection.Y < 0f && !ceilingRay.IsColliding() && _vaultCast.IsColliding() && _vaultCheck.IsColliding()
				&& !stepCast.IsColliding() && (angleToFloor > 80f || Mathf.IsZeroApprox(angleToFloor)))
		{
			if (vaultElevation > minElevation || IsOnWall())
			{
				vaultPoint = _vaultPoint;
				DebugDraw3D.DrawSphere(_vaultPoint, 0.25f, Colors.Red);

				return true;
			}
			
		}

		return false;
	}

	public bool CheckLadder()
	{
		if (currentLadder == null)
			return false;

		Node3D area = currentLadder.GetNode<Node3D>("LadderArea/LadderCollider");

		if (area == null)
			return false;

		if (!isLadder)
			return false;

		// Check if player is facing ladder
		Vector3 wallDirection = default;
		Vector3 wallPoint = default;

		// Get forward ray
		bool forwardRay = SendRayInDirection(-GlobalBasis.Z, 0.5f, out Vector3 rayNormal, out Vector3 rayPoint, out GodotObject collider);

		float dotCollision = Mathf.Abs(rayNormal.Dot(Vector3.Up));

		// Threshold to how slanted the wall can be
		if (dotCollision < 0.3f)
		{
			Vector3 playerForward = -GlobalBasis.Z;
			
			float forwardAngle = Mathf.RadToDeg(playerForward.AngleTo(-rayNormal));

			wallDirection = GlobalBasis.X.Cross(rayNormal).Normalized();
			wallDirection = new Vector3(
				Mathf.Abs(wallDirection.X),
				Mathf.Abs(wallDirection.Y),
				Mathf.Abs(wallDirection.Z)
			);

			Vector3 ladderZOffset = -rayNormal * -0.5f;

			float nearestBarHeight = Mathf.Round(GlobalPosition.Y / currentLadder.BarSpacing) * currentLadder.BarSpacing + 1f;
			wallPoint = new Vector3(area.GlobalPosition.X, nearestBarHeight, area.GlobalPosition.Z);

			// Check if not facing completely side ways to the wall and in general direction
			if (forwardAngle > 0 && forwardAngle < 20f)
			{
				// Check if player velocity is more than or equal to 0
				if (inputDirection.Y < 0f)
				{
					DebugDraw3D.DrawArrow(wallPoint, wallPoint + (wallDirection * 3f), Colors.Red, 0.2f, false, 5f);

					
					GlobalPosition = wallPoint + ladderZOffset;
					return true;
				}
			}
		}

		return false;
	}

	private void ResetJumps() => _jumpsDone = 0;

	public bool IsRunningUpSlope()
	{
		float dot = GetFloorNormal().Dot(-Transform.Basis.Z);
		
		if (dot < 0f)
			return true;
		else 
			return false;
	}
	public bool CheckWall(out KinematicCollision3D collision, out String direction)
	{
		int count = GetSlideCollisionCount();
		direction = String.Empty;
		collision = default(KinematicCollision3D);

		if (count <= 0)
			return false;
		
		if (inputDirection.Y >= 0)
			return false;
		
		List<KinematicCollision3D> collisions = new List<KinematicCollision3D>();
		
		for (int i = 0; i < count; i++)
		{
			collisions.Add(GetSlideCollision(i));
		}

		foreach (KinematicCollision3D c in collisions)
		{
			Vector3 collisionNormal = c.GetNormal();
			Vector3 collisionPoint = c.GetPosition();

			float dotCollision = Mathf.Abs(collisionNormal.Dot(Vector3.Up)); // Get the dot product to see if the wall is side ways

			if (dotCollision < 0.1f)
			{
				Vector3 playerForward = this.GlobalBasis.Z;
				float angleToWall = Mathf.RadToDeg(playerForward.AngleTo(collisionNormal));
				float signedAngle = Mathf.RadToDeg(playerForward.SignedAngleTo(collisionNormal, Vector3.Up));			

				// Check if camera is facing somewhat in that direction
				if (angleToWall < 105f && angleToWall > 25f)
				{
					//DebugDraw3D.DrawSquare(c.GetPosition(), 0.1f, Colors.Blue);

					direction = Mathf.Sign(signedAngle) > 0 ? "Left" : "Right";
					collision = c;
					
					//GD.Print(direction);
					
					return true;
				}
			}
		}

		return false;
	}

	public bool CheckVerticalWall(out Vector3 wallDirection, out Vector3 wallPoint)
	{
		wallDirection = default;
		wallPoint = default;

		// Get forward ray
		bool forwardRay = SendRayInDirection(-GlobalBasis.Z, 0.5f, out Vector3 rayNormal, out Vector3 rayPoint);

		if (forwardRay)
		{
			float dotCollision = Mathf.Abs(rayNormal.Dot(Vector3.Up));

			// Threshold to how slanted the wall can be
			if (dotCollision < 0.3f)
			{
				Vector3 cameraUp = _camera.GlobalBasis.Y;
				Vector3 playerForward = GlobalBasis.Z;
				
				float upAngleDot = rayNormal.Dot(cameraUp);
				float forwardAngle = Mathf.RadToDeg(playerForward.AngleTo(rayNormal));

				wallDirection = GlobalBasis.X.Cross(rayNormal).Normalized();
				wallDirection = new Vector3(
					Mathf.Abs(wallDirection.X),
					Mathf.Abs(wallDirection.Y),
					Mathf.Abs(wallDirection.Z)
				);

				wallPoint = rayPoint;

				// Check if not facing completely side ways to the wall and in general direction
				if (forwardAngle > 0 && forwardAngle < 20f)
				{
					// Check if camera is facing somewhat upwards
					if (upAngleDot > 0.2f)
					{
						// Check if player velocity is more than or equal to 0
						if (Velocity.Y >= 0)
						{
							DebugDraw3D.DrawArrow(wallPoint, wallPoint + (wallDirection * 1f), Colors.Aqua, 0.2f);
							return true;
						}
					}
				}
			}
		}

		return false;
	}

	public bool SendRayInDirection(Vector3 direction, float range, out Vector3 rayNormal, out Vector3 rayPoint)
	{
		// Send ray in direction of wall
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

        Vector3 rayOrigin = _camera.GlobalPosition;
        Vector3 rayEnd = rayOrigin + (direction * range);

		rayNormal = default(Vector3);
		rayPoint = default(Vector3);

		uint layerMask = (1 << 1)  | (1 << 4);

        PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd, layerMask);
		parameters.HitBackFaces = false;
		parameters.HitFromInside = false;

        var rayArray = spaceState.IntersectRay(parameters);

        if (rayArray.ContainsKey("collider"))
        {
			rayArray.TryGetValue("normal", out Variant normal);
			rayArray.TryGetValue("position", out Variant position);
			rayArray.TryGetValue("collider", out Variant colliderVariant);

			DebugDraw3D.DrawArrow(rayOrigin, rayEnd, Colors.GreenYellow, 0.2f);

			rayNormal = normal.AsVector3();
			rayPoint = position.AsVector3();

            return true;
        }

		return false;
	}

	public bool SendRayInDirection(Vector3 direction, float range, out Vector3 rayNormal, out Vector3 rayPoint, out GodotObject collider)
	{
		// Send ray in direction of wall
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

        Vector3 rayOrigin = _camera.GlobalPosition;
        Vector3 rayEnd = rayOrigin + (direction * range);

		rayNormal = default(Vector3);
		rayPoint = default(Vector3);
		collider = default(GodotObject);

		uint layerMask = (1 << 1)  | (1 << 4);

        PhysicsRayQueryParameters3D parameters = PhysicsRayQueryParameters3D.Create(rayOrigin, rayEnd, layerMask);
		parameters.HitBackFaces = false;
		parameters.HitFromInside = false;
		parameters.CollideWithAreas = true;
		parameters.CollideWithBodies = true;

        var rayArray = spaceState.IntersectRay(parameters);

        if (rayArray.ContainsKey("collider"))
        {
			rayArray.TryGetValue("normal", out Variant normal);
			rayArray.TryGetValue("position", out Variant position);
			rayArray.TryGetValue("collider", out Variant colliderVariant);

			DebugDraw3D.DrawArrow(rayOrigin, rayEnd, Colors.GreenYellow, 0.2f);

			rayNormal = normal.AsVector3();
			rayPoint = position.AsVector3();
			collider = colliderVariant.AsGodotObject();

            return true;
        }

		return false;
	}
}

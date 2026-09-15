public partial class PlayerMovement
{
	public void Crouch(double delta)
	{
		Vector3 depth;

		// Crouching
		_standingCollider.Disabled = true;
		_crouchingCollider.Disabled = false;

		depth = new Vector3(_head.Position.X, _initialDepth - _crouchingDepth, _head.Position.Z);
		_head.Position = _head.Position.Lerp(depth, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
	}

	public void Stand(double delta)
	{
		Vector3 depth;

		_standingCollider.Disabled = false;
		_crouchingCollider.Disabled = true;

		depth = new Vector3(_head.Position.X, _initialDepth, _head.Position.Z);
		_head.Position = _head.Position.Lerp(depth, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));

		// reduce momentum here and place momentum on top of sprintingSpeed
		if (FSM.CurrentState is not PlayerSlide && momentum >= 0)
			momentum -= (float)delta * (slideSpeed / 2);
	}
}

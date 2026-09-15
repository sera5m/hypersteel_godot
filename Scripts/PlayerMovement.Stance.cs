using Godot;
using System;

public partial class PlayerMovement
{
	public void Crouch(double delta)
	{
		_standingCollider.Disabled = true;
		_crouchingCollider.Disabled = false;
		Vector3 depth = new Vector3(_head.Position.X, _initialDepth - _crouchingDepth, _head.Position.Z);
		_head.Position = _head.Position.Lerp(depth, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
	}

	public void Stand(double delta)
	{
		_standingCollider.Disabled = false;
		_crouchingCollider.Disabled = true;
		Vector3 depth = new Vector3(_head.Position.X, _initialDepth, _head.Position.Z);
		_head.Position = _head.Position.Lerp(depth, 1.0f - Mathf.Pow(0.5f, (float)delta * lerpSpeed));
		if (FSM.CurrentState is not PlayerSlide && momentum >= 0)
			momentum -= (float)delta * (slideSpeed / 2);
	}
}

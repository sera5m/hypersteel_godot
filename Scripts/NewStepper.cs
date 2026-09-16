using Godot;

public partial class NewStepper : Node3D
{
	[ExportCategory("Stepper")]
	[Export] private bool _enableStepper = true;
	[Export] private RayCast3D _stepperRay;
	[Export] private CharacterBody3D _characterBody;
	[Export] private PlayerMovement _playerMovement;

	[ExportSubgroup("Stepper projection")]
	[Export(PropertyHint.Range, "0.05f, 2f")] private float _projectionMaxDistance;
	[Export(PropertyHint.Range, "0.1f, 3f")] private float _stepHeight;

	private float _stepRayDistance;

	[Export(PropertyHint.Range, "0.01f, 0.2f")] private float _maxElevation = 0.02f;

	[ExportSubgroup("Projection Smoother")]
	[Export] private float _smoothingSpeed = 5.0f;

	[ExportSubgroup("Gizmos")]
[Export] private MeshInstance3D _pointGizmo;

	private Vector3 _currentProjection = Vector3.Zero;

	public override void _Ready()
	{
		float third = _characterBody != null ? SourceMove.CapsuleHeight(_characterBody) / 3f : 0.6f;
		if (_stepHeight < third)
			_stepHeight = third;
		if (_characterBody != null)
			_characterBody.FloorSnapLength = _stepHeight;

		_stepRayDistance = _stepHeight * 4f;
		if (_stepperRay != null)
		{
			_stepperRay.Position = new Vector3(0f, Mathf.Abs(_stepHeight), 0f);
			_stepperRay.TargetPosition = new Vector3(0f, -_stepRayDistance, 0f);
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_enableStepper || _characterBody == null || _playerMovement == null)
			return;

		Vector3 rawProjectedXZ = new Vector3(_playerMovement.inputDirection.X, 0f, _playerMovement.inputDirection.Y);
		_currentProjection = _currentProjection.Lerp(rawProjectedXZ, _smoothingSpeed * (float)delta);
		Vector3 inputProjectionPoint = _projectionMaxDistance * _currentProjection;
		Position = new Vector3(inputProjectionPoint.X, _stepperRay != null ? _stepperRay.Position.Y : 0f, inputProjectionPoint.Z);

		if (GetStep(out Vector3 projectionPoint))
		{
			if (_pointGizmo != null) _pointGizmo.GlobalPosition = projectionPoint;
			Vector3 stepPosition = _characterBody.GlobalPosition;
			if (stepPosition.Y > projectionPoint.Y)
				stepPosition = projectionPoint;
			else
				stepPosition.Y = projectionPoint.Y;
			_characterBody.GlobalPosition = stepPosition;
		}
		else if (_pointGizmo != null)
		{
			_pointGizmo.GlobalPosition = _characterBody.Position;
		}
	}

	private bool GetStep(out Vector3 projectionPoint)
	{
		projectionPoint = default;
		if (!_characterBody.IsOnFloor()) return false;
		if (_stepperRay == null || !_stepperRay.IsColliding()) return false;

		Vector3 stepPoint = _stepperRay.GetCollisionPoint();
		Vector3 stepNormal = _stepperRay.GetCollisionNormal();
		if (stepNormal.Y < 0.99f) return false;

		float elevation = Mathf.Abs(_characterBody.GlobalPosition.Y - stepPoint.Y);
		if (elevation < _maxElevation) return false;
		if (elevation > _stepHeight) return false;

		projectionPoint = stepPoint;
		return true;
	}
}

using Godot;

public partial class MotionBlur : MeshInstance3D
{
	[Export] public float minSpeed = 25f;
	[Export] public float intensity = 0.06f;

	private Camera3D _cam;
	private Vector3 _camPosPrev = Vector3.Zero;
	private Quaternion _camRotPrev = Quaternion.Identity;
	private ShaderMaterial _material;
	private float _playerSpeed;

	public override void _Ready()
	{
		_cam = PostProcessingManager.Instance?.mainCamera;
		_material = MaterialOverride as ShaderMaterial;
		PlayerMovement.VelocityChange += OnVel;
	}

	public override void _ExitTree()
	{
		PlayerMovement.VelocityChange -= OnVel;
	}

	void OnVel(Vector3 v) => _playerSpeed = v.Length();

	public override void _Process(double delta)
	{
		if (_cam == null || _material == null) return;

		Vector3 velocity = _cam.GlobalTransform.Origin - _camPosPrev;
		Quaternion camRot = _cam.GlobalTransform.Basis.GetRotationQuaternion();
		Quaternion camRotDiff = camRot - _camRotPrev;
		Quaternion angleVelocity = camRotDiff * 2.0f * Conjugate(camRot);
		Vector3 angleVelVector = new Vector3(Mathf.Abs(angleVelocity.X), Mathf.Abs(angleVelocity.Y), angleVelocity.Z);

		float scale = _playerSpeed >= minSpeed ? intensity : 0f;
		_material.SetShaderParameter("angular_velocity", angleVelVector * scale);
		_material.SetShaderParameter("linear_velocity", velocity * scale);

		_camPosPrev = _cam.GlobalTransform.Origin;
		_camRotPrev = camRot;
	}

	static Quaternion Conjugate(Quaternion quat) => new Quaternion(-quat.X, -quat.Y, -quat.Z, quat.W);
}

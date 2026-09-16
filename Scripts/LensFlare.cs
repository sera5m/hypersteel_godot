using Godot;

public partial class LensFlare : ColorRect
{
	public bool sunBlocked;
	private Vector3 _effSunDirection;
	private float _adjustTime = 0.25f;
	private ShaderMaterial _material;
	private DirectionalLight3D _directionalLight;
	private Camera3D _camera;
	static readonly Vector3 SoftTint = new Vector3(0.28f, 0.28f, 0.2f);

	public override void _Ready()
	{
		_material = Material as ShaderMaterial;
		_directionalLight = PostProcessingManager.Instance?.mainLight;
		_camera = PostProcessingManager.Instance?.mainCamera;
	}

	public override void _Process(double delta)
	{
		if (_directionalLight == null || _camera == null || _material == null) return;

		_effSunDirection = (-_directionalLight.GlobalTransform.Basis.Z * Mathf.Max(_camera.Near, 1.0f)).Normalized();
		_effSunDirection += _camera.GlobalTransform.Origin;

		if (sunBlocked || _camera.IsPositionBehind(_effSunDirection))
		{
			Fade(Vector3.Zero);
			return;
		}

		if (Visible)
		{
			Fade(SoftTint);
			Vector2 unprojectedSunPos = _camera.UnprojectPosition(_effSunDirection);
			_material.SetShaderParameter("sun_position", unprojectedSunPos);
		}
	}

	void Fade(Vector3 tint)
	{
		Tween tween = CreateTween();
		tween.TweenProperty(_material, "shader_parameter/tint", tint, _adjustTime);
	}
}

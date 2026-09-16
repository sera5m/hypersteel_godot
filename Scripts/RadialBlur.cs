using Godot;

public partial class RadialBlur : ColorRect
{
	[Export] public float power = 0.02f;
	private ShaderMaterial _material;

	public override void _Ready()
	{
		_material = Material as ShaderMaterial;
		if (_material == null) return;
		power = Mathf.Min((float)_material.GetShaderParameter("blur_power"), 0.03f);
	}

	public override void _Process(double delta)
	{
		_material?.SetShaderParameter("blur_power", power);
	}
}

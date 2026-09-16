using Godot;

public partial class Vignette : ColorRect
{
	[Export] public float vIntensity { set; get; } = 0.12f;
	[Export] public float vOpacity { set; get; } = 0.08f;
	[Export] public Color vColor { set; get; } = new Color(0f, 0f, 0f, 1f);

	private ShaderMaterial _material;

	public override void _Ready()
	{
		_material = Material as ShaderMaterial;
		if (_material == null) return;
		// Cap whatever the .tres authored — scene values were too hot.
		vIntensity = Mathf.Min((float)_material.GetShaderParameter("vignette_intensity"), 0.15f);
		vOpacity = Mathf.Min((float)_material.GetShaderParameter("vignette_opacity"), 0.1f);
		vColor = (Color)_material.GetShaderParameter("vignette_rgb");
	}

	public override void _Process(double delta)
	{
		if (_material == null) return;
		_material.SetShaderParameter("vignette_intensity", vIntensity);
		_material.SetShaderParameter("vignette_opacity", vOpacity);
		_material.SetShaderParameter("vignette_rgb", vColor);
	}
}

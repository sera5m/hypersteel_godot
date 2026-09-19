using Godot;

namespace Hypersteel.Weather;

public struct WindSample
{
	public Vector3 Direction;
	public WindTier Tier;
	public bool Occluded;
	public bool FromBake;

	public static WindSample Still => new()
	{
		Direction = Vector3.Zero,
		Tier = WindTier.Still,
		Occluded = true,
		FromBake = false,
	};

	public float ShoveMul => Tier switch
	{
		WindTier.Breeze => 0.05f,
		WindTier.Wind => 0.2f,
		WindTier.Gale => 0.55f,
		WindTier.Storm => 1f,
		_ => 0f,
	};
}

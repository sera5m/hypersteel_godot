using Godot;

namespace Hypersteel.Weather;

public static class WeatherGameplayEffects
{
	public const float HullMassIgnoreBelowGale = 800f;

	public static Vector3 ProjectileLead(Vector3 velocity, WindSample wind, float dt)
	{
		if (wind.Tier < WindTier.Gale || wind.Occluded) return velocity;
		return velocity + wind.Direction * wind.ShoveMul * 8f * dt;
	}

	public static Vector3 BodyShove(float mass, WindSample wind, float dt)
	{
		if (wind.Occluded || wind.Tier < WindTier.Wind) return Vector3.Zero;
		if (mass >= HullMassIgnoreBelowGale && wind.Tier < WindTier.Storm) return Vector3.Zero;
		return wind.Direction * wind.ShoveMul * 400f / Mathf.Max(mass, 1f) * dt;
	}

	public static bool LaunchesUnanchored(WindSample wind, float mass)
		=> !wind.Occluded && wind.Tier == WindTier.Storm && mass < HullMassIgnoreBelowGale;
}

namespace Hypersteel.Weather;

/// <summary>Each bake bounce drops one tier. Storm launches physics. Breeze does not.</summary>
public enum WindTier
{
	Still = 0,
	Breeze = 1,
	Wind = 2,
	Gale = 3,
	Storm = 4,
}

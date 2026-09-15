namespace Hypersteel.Damage;

/// <summary>
/// Godot 3D physics layers, 1-based in the editor, 0-based bits here.
/// Project Settings → Layer Names → 3D:
/// 1 World, 2 PawnPhys, 3 Damage.
/// </summary>
public static class CollisionLayers
{
	public const uint World = 1u << 0;
	public const uint PawnPhys = 1u << 1;
	public const uint Damage = 1u << 2;

	public const uint Trace = World | Damage;
}

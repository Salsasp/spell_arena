using Godot;

public static class SortLayers
{
	public const int Floors = 0;
	public const int Walls = 1;
	public const int Pitfalls = 2;
	public const int Projectiles = 3;
	public const int Entities = 4;
	public const int Wands = 5;

	public static void Apply(CanvasItem item, int layer, bool relative = false)
	{
		if (item == null)
		{
			return;
		}

		item.ZAsRelative = relative;
		item.ZIndex = layer;
	}
}

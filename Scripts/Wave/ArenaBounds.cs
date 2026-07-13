using Godot;

public partial class ArenaBounds : Node2D
{
	[Export] public Vector2 ArenaHalfSize { get; set; } = new(300, 260);
	[Export] public float SpawnMargin { get; set; } = 90f;
	[Export] public float MinSpawnDistanceFromPlayer { get; set; } = 120f;

	public Rect2 GetArenaRect()
	{
		return new Rect2(GlobalPosition - ArenaHalfSize, ArenaHalfSize * 2);
	}

	public Vector2 ClampPosition(Vector2 position)
	{
		Rect2 arena = GetArenaRect();
		return new Vector2(
			Mathf.Clamp(position.X, arena.Position.X, arena.End.X),
			Mathf.Clamp(position.Y, arena.Position.Y, arena.End.Y)
		);
	}

	public Vector2 GetSpawnPosition(Vector2 playerPosition)
	{
		Rect2 outer = GetArenaRect().Grow(SpawnMargin);

		for (int attempt = 0; attempt < 12; attempt++)
		{
			Vector2 spawnPos = GetRandomEdgePoint(outer);
			if (spawnPos.DistanceTo(playerPosition) >= MinSpawnDistanceFromPlayer)
			{
				return spawnPos;
			}
		}

		return GetRandomEdgePoint(outer);
	}

	private static Vector2 GetRandomEdgePoint(Rect2 rect)
	{
		int edge = (int)(GD.Randi() % 4);

		return edge switch
		{
			0 => new Vector2((float)GD.RandRange(rect.Position.X, rect.End.X), rect.Position.Y),
			1 => new Vector2((float)GD.RandRange(rect.Position.X, rect.End.X), rect.End.Y),
			2 => new Vector2(rect.Position.X, (float)GD.RandRange(rect.Position.Y, rect.End.Y)),
			_ => new Vector2(rect.End.X, (float)GD.RandRange(rect.Position.Y, rect.End.Y)),
		};
	}

	public override void _Draw()
	{
		Rect2 arena = GetArenaRect();
		DrawRect(arena, new Color(0.2f, 0.8f, 0.3f, 0.08f), true);
		DrawRect(arena, new Color(0.2f, 0.8f, 0.3f, 0.35f), false, 2f);
	}
}

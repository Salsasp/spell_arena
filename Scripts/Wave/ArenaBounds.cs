using Godot;

public partial class ArenaBounds : Node2D
{
	[Export] public NodePath BoundsTileMapLayerPath { get; set; }
	[Export] public Vector2 ArenaHalfSize { get; set; } = new(300, 260);
	[Export] public float SpawnMargin { get; set; } = 90f;
	[Export] public float MinSpawnDistanceFromPlayer { get; set; } = 120f;
	[Export] public float SpawnOutsideCameraMargin { get; set; } = 48f;

	public override void _Ready()
	{
		if (BoundsTileMapLayerPath == null || BoundsTileMapLayerPath.IsEmpty)
		{
			return;
		}

		TileMapLayer layer = GetNodeOrNull<TileMapLayer>(BoundsTileMapLayerPath);
		if (layer == null)
		{
			GD.PushWarning($"{Name}: BoundsTileMapLayerPath did not resolve to a TileMapLayer.");
			return;
		}

		InitializeFromTileMap(layer);
	}

	private void InitializeFromTileMap(TileMapLayer layer)
	{
		Rect2I used = layer.GetUsedRect();
		if (used.Size == Vector2I.Zero)
		{
			GD.PushWarning($"{Name}: TileMapLayer has no tiles; using manual ArenaHalfSize.");
			return;
		}

		Vector2 topLeft = layer.ToGlobal(layer.MapToLocal(used.Position));
		Vector2 bottomRight = layer.ToGlobal(layer.MapToLocal(used.End));
		Vector2 size = bottomRight - topLeft;

		GlobalPosition = topLeft + size / 2f;
		ArenaHalfSize = size / 2f;
	}

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

	public Vector2 GetSpawnPosition(Vector2 playerPosition, Camera2D camera = null)
	{
		Rect2 outer = GetArenaRect().Grow(SpawnMargin);
		Rect2? blockedRect = TryGetCameraBlockedRect(camera);

		for (int attempt = 0; attempt < 32; attempt++)
		{
			Vector2 spawnPos = GetRandomEdgePoint(outer);
			if (IsValidSpawnPosition(spawnPos, playerPosition, blockedRect))
			{
				return spawnPos;
			}
		}

		return GetBestEdgeSpawnPoint(outer, playerPosition, blockedRect);
	}

	private Rect2? TryGetCameraBlockedRect(Camera2D camera)
	{
		if (camera == null || !GodotObject.IsInstanceValid(camera))
		{
			return null;
		}

		return GetCameraVisibleRect(camera).Grow(SpawnOutsideCameraMargin);
	}

	public static Rect2 GetCameraVisibleRect(Camera2D camera)
	{
		Viewport viewport = camera.GetViewport();
		Vector2 viewportSize = viewport.GetVisibleRect().Size;
		Vector2 visibleSize = viewportSize / camera.Zoom;
		Vector2 center = camera.GetScreenCenterPosition();
		return new Rect2(center - visibleSize / 2f, visibleSize);
	}

	private bool IsValidSpawnPosition(Vector2 spawnPos, Vector2 playerPosition, Rect2? blockedRect)
	{
		if (spawnPos.DistanceTo(playerPosition) < MinSpawnDistanceFromPlayer)
		{
			return false;
		}

		if (blockedRect.HasValue && blockedRect.Value.HasPoint(spawnPos))
		{
			return false;
		}

		return true;
	}

	private Vector2 GetBestEdgeSpawnPoint(Rect2 outer, Vector2 playerPosition, Rect2? blockedRect)
	{
		Vector2 best = GetRandomEdgePoint(outer);
		float bestScore = float.MinValue;

		for (int edge = 0; edge < 4; edge++)
		{
			for (int sample = 0; sample < 8; sample++)
			{
				Vector2 spawnPos = GetEdgePoint(outer, edge, sample / 7f);
				float score = ScoreSpawnPosition(spawnPos, playerPosition, blockedRect);
				if (score > bestScore)
				{
					bestScore = score;
					best = spawnPos;
				}
			}
		}

		return best;
	}

	private float ScoreSpawnPosition(Vector2 spawnPos, Vector2 playerPosition, Rect2? blockedRect)
	{
		float score = spawnPos.DistanceTo(playerPosition);

		if (blockedRect.HasValue && blockedRect.Value.HasPoint(spawnPos))
		{
			score -= 10000f;
		}

		if (spawnPos.DistanceTo(playerPosition) < MinSpawnDistanceFromPlayer)
		{
			score -= 5000f;
		}

		return score;
	}

	private static Vector2 GetEdgePoint(Rect2 rect, int edge, float t)
	{
		t = Mathf.Clamp(t, 0f, 1f);

		return edge switch
		{
			0 => new Vector2(Mathf.Lerp(rect.Position.X, rect.End.X, t), rect.Position.Y),
			1 => new Vector2(Mathf.Lerp(rect.Position.X, rect.End.X, t), rect.End.Y),
			2 => new Vector2(rect.Position.X, Mathf.Lerp(rect.Position.Y, rect.End.Y, t)),
			_ => new Vector2(rect.End.X, Mathf.Lerp(rect.Position.Y, rect.End.Y, t)),
		};
	}

	private static Vector2 GetRandomEdgePoint(Rect2 rect)
	{
		return GetEdgePoint(rect, (int)(GD.Randi() % 4), (float)GD.Randf());
	}

	public override void _Draw()
	{
		Rect2 arena = GetArenaRect();
		DrawRect(arena, new Color(0.2f, 0.8f, 0.3f, 0.08f), true);
		DrawRect(arena, new Color(0.2f, 0.8f, 0.3f, 0.35f), false, 2f);

		Rect2 spawnRing = arena.Grow(SpawnMargin);
		DrawRect(spawnRing, new Color(0.9f, 0.5f, 0.1f, 0.06f), false, 1f);
	}
}

using Godot;
using System;

public partial class Pitfall : Node2D
{
	[Export] public Sprite2D PitfallSprite;
	[Export] public Sprite2D PitfallBottomLipSprite;
	[Export] public Area2D FallArea;
	[Export] public Area2D SwipeArea;
	[Export] public Node2D SkeletalHand;
	[Export] public int PitfallDamage = 20;
	[Export] public float RespawnOffset = 30;
	[Export] public Marker2D FallStartPoint;

	public override void _Ready()
	{
		FallArea.BodyEntered += OnFallAreaEntered;
		SwipeArea.BodyEntered += OnSwipeAreaEntered;
	}

	private void OnSwipeAreaEntered(Node2D body)
	{
		
	}

	private void OnFallAreaEntered(Node2D body)
	{
		if (body is IEntity entity)
		{
			entity.Fall(this);
		}
	}

	public Vector2 GetFallRespawnPosition(Node2D entity)
	{
		CollisionShape2D shape2D = FallArea.GetNode<CollisionShape2D>("CollisionShape2D");
		var rectShape = (RectangleShape2D)shape2D.Shape;
		Vector2 extents = rectShape.Size / 2f;

		// Work in the shape's local space
		Vector2 localPos = entity.GlobalPosition - shape2D.GlobalPosition;

		// Signed distance to each edge: positive = inside the rect, negative = outside past that edge.
		// The smallest value is the edge the entity is closest to (or entered through).
		float dLeft = localPos.X + extents.X;
		float dRight = extents.X - localPos.X;
		float dTop = localPos.Y + extents.Y;
		float dBottom = extents.Y - localPos.Y;
		float dMin = Mathf.Min(Mathf.Min(dLeft, dRight), Mathf.Min(dTop, dBottom));

		// Keep the perpendicular axis clamped to the rect so the respawn stays beside the entry point
		float clampedX = Mathf.Clamp(localPos.X, -extents.X, extents.X);
		float clampedY = Mathf.Clamp(localPos.Y, -extents.Y, extents.Y);

		// Push out past the nearest edge, always including the offset
		if (dMin == dLeft)
		{
			clampedX = -extents.X - RespawnOffset;
		}
		else if (dMin == dRight)
		{
			clampedX = extents.X + RespawnOffset;
		}
		else if (dMin == dTop)
		{
			clampedY = -extents.Y - RespawnOffset;
		}
		else
		{
			clampedY = extents.Y + RespawnOffset;
		}

		// Convert back to global space
		return shape2D.GlobalPosition + new Vector2(clampedX, clampedY);
	}
}

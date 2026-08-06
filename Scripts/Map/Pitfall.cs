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

		float clampedX = Mathf.Clamp(localPos.X, -extents.X, extents.X);
		float clampedY = Mathf.Clamp(localPos.Y, -extents.Y, extents.Y);

		// If the point is inside the rectangle, snap it to the closest edge
		if (MathF.Abs(localPos.X) < extents.X && MathF.Abs(localPos.Y) < extents.Y)
		{
			float dX = extents.X - MathF.Abs(localPos.X);
			float dY = extents.Y - MathF.Abs(localPos.Y);

			if (dX < dY)
			{
				clampedX = extents.X * MathF.Sign(localPos.X);
				clampedX += RespawnOffset * MathF.Sign(localPos.X);
			}
			else
			{
				clampedY = extents.Y * MathF.Sign(localPos.Y);
				clampedY += RespawnOffset * MathF.Sign(localPos.Y);
			}
		}

		// Convert back to global space
		return shape2D.GlobalPosition + new Vector2(clampedX, clampedY);
	}
}

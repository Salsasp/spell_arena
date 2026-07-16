using Godot;

public partial class SpellMovementComponent : Component
{
	[Export] public Sprite2D Sprite;
	[Export] public float Speed = 50f;
	[Export] public float HomingTurnSpeed = 4f;

	public Vector2 Velocity = Vector2.Zero;
	public Vector2 Direction = Vector2.Zero;

	private bool homingEnabled;
	private float homingTurnSpeed;

	public void SetHomingEnabled(bool enabled, float turnSpeed)
	{
		homingEnabled = enabled;
		homingTurnSpeed = turnSpeed;
	}

	public void Update(double delta = 0)
	{
		if (homingEnabled)
		{
			ApplyHoming((float)delta);
		}

		if (Direction != Vector2.Zero)
		{
			Velocity = Direction * Speed;
		}
	}

	private void ApplyHoming(float delta)
	{
		Enemy target = FindNearestEnemy();
		if (target == null)
		{
			return;
		}

		Node2D projectile = GetParent<Node2D>();
		Vector2 toTarget = projectile.GlobalPosition.DirectionTo(target.GlobalPosition);

		if (Direction == Vector2.Zero)
		{
			Direction = toTarget;
		}
		else
		{
			float newAngle = Mathf.RotateToward(Direction.Angle(), toTarget.Angle(), homingTurnSpeed * delta);
			Direction = Vector2.FromAngle(newAngle);
		}

		Area2D projArea = projectile.GetNodeOrNull<Area2D>("ProjArea2D");
		if (projArea != null)
		{
			projArea.Rotation = Direction.Angle();
		}
	}

	private Enemy FindNearestEnemy()
	{
		Vector2 pos = GetParent<Node2D>().GlobalPosition;
		Enemy nearest = null;
		float minDistSq = float.MaxValue;

		foreach (Node node in GetTree().GetNodesInGroup("enemies"))
		{
			if (node is not Enemy enemy || !GodotObject.IsInstanceValid(enemy))
			{
				continue;
			}

			float distSq = pos.DistanceSquaredTo(enemy.GlobalPosition);
			if (distSq < minDistSq)
			{
				minDistSq = distSq;
				nearest = enemy;
			}
		}

		return nearest;
	}
}

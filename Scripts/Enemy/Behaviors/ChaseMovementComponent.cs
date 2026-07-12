using Godot;

public partial class ChaseMovementComponent : AIMovementBehaviorComponent
{
	public override void Update(double delta)
	{
		if (context == null || !context.IsPlayerValid)
		{
			movement.direction = Vector2.Zero;
			return;
		}

		movement.direction = owner.GlobalPosition
			.DirectionTo(context.Player.GlobalPosition)
			.Normalized();
	}
}

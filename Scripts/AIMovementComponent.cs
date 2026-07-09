using Godot;
using System;

public partial class AIMovementComponent : Node2D
{
	private Player player;
	private MovementComponent movementComponent;

	public override void _Ready()
	{
	}

	public void Initialize(MovementComponent movementComponent, Player player)
	{
		this.movementComponent = movementComponent;
		this.player = player;
	}

	public override void _PhysicsProcess(double delta)
	{
		Pathfind(player);
	}

	public void Pathfind(Node2D target)
	{
		Vector2 targetPos = target.GlobalPosition;
		Vector2 toTargetDir = GlobalPosition.DirectionTo(target.GlobalPosition);
	}
}

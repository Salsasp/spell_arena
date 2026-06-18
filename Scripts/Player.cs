using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private InputComponent inputComponent;
	private MovementComponent movementComponent;
	private ProjectileSpawner projectileSpawner;

    public override void _Ready()
	{
		projectileSpawner= GetNode<ProjectileSpawner>("../ProjectileSpawner");
		inputComponent = GetNode<InputComponent>("InputComponent");
    	movementComponent = GetNode<MovementComponent>("MovementComponent");
	}

	public override void _PhysicsProcess(double delta)
	{
		inputComponent.Update();
		// TODO: replace GlobalPosition with the GlobalPosition of a custom point2D
		Vector2 projDir = GlobalPosition.DirectionTo(inputComponent.MousePos).Normalized();
		if (inputComponent.LeftClickPressed)
		{
			projectileSpawner.SpawnProjectile(GlobalPosition, projDir);
		}
		movementComponent.direction = inputComponent.MoveDir;
		movementComponent.Update(delta);
	}

}

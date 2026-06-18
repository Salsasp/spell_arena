using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private InputComponent inputComponent;
	private MovementComponent movementComponent;

    public override void _Ready()
	{
		inputComponent = GetNode<InputComponent>("InputComponent");
    	movementComponent = GetNode<MovementComponent>("MovementComponent");
	}

	public override void _PhysicsProcess(double delta)
	{
		inputComponent.Update();
		movementComponent.direction = inputComponent.moveDir;
		movementComponent.Update(delta);
	}

}

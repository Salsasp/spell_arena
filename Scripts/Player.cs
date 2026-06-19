using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private InputComponent inputComponent;
	private MovementComponent movementComponent;
	private ProjectileSpawner projectileSpawner;
	private AnimationComponent animationComponent;
	private Wand wand;

    public override void _Ready()
	{
		projectileSpawner= GetNode<ProjectileSpawner>("../ProjectileSpawner");
		inputComponent = GetNode<InputComponent>("InputComponent");
    	movementComponent = GetNode<MovementComponent>("MovementComponent");
		animationComponent = GetNode<AnimationComponent>("AnimationComponent");
		wand = GetNode<Wand>("Wand");
	}

	public override void _PhysicsProcess(double delta)
	{
		inputComponent.Update();
		// TODO: replace GlobalPosition with the GlobalPosition of a custom point2D
		Vector2 EndOfWandPos = wand.GetEndOfWandPos();
		Vector2 projDir = EndOfWandPos.DirectionTo(inputComponent.MousePos).Normalized();
		
		if (inputComponent.LeftClickJustPressed)
		{
			projectileSpawner.SpawnProjectile(EndOfWandPos, projDir);
		}

		wand.RotateWand(inputComponent);
		movementComponent.direction = inputComponent.MoveDir;
		movementComponent.Update(delta);
		animationComponent.Update(movementComponent);
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public override void _Draw()
    {
		if (inputComponent.LeftClickPressed)
		{
			//DrawLine(wand.GetEndOfWandPos(), inputComponent.MousePos, Colors.Red);
		}
    }

}

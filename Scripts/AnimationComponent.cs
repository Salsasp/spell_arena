using Godot;
using System;

public partial class AnimationComponent : Node2D
{
	[Export] public AnimatedSprite2D animatedSprite;
	[Export] public AnimationPlayer animationPlayer;
	[Export] public Sprite2D sprite;
	[Export] public bool directionalSprites;
	private string animationState = "idle";

	public void Update(MovementComponent movementComponent)
	{
		// Ignore animated movement if npc is stationary
		if (directionalSprites)
		{
			if (movementComponent.direction.Length() > 0)
			{
				Vector2 direction = movementComponent.direction.Round();
				switch (direction)
				{
					case var v when v == Vector2.Right: animationState = "walk_right"; break;
					case var v when v == Vector2.Left: animationState = "walk_left"; break;
					case var v when v == Vector2.Up: animationState = "walk_up"; break;
					case var v when v == Vector2.Down: animationState = "walk_down"; break;
				}
			}
			animatedSprite.Play(animationState);
		}
		
	}

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}

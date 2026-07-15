using Godot;
using System;

public partial class AnimationComponent : Node2D
{
	[Export] public AnimatedSprite2D animatedSprite;
	[Export] public AnimationPlayer animationPlayer;
	[Export] public bool directionalSprites;
	//[Export] public bool isPlayer;
	private string animationState = "idle";

	public void Update(MovementComponent movementComponent = null)
	{
		// Ignore animated movement if npc doesn't move / non player animation
		if (directionalSprites && movementComponent != null)
		{
			Vector2 direction = movementComponent.mouseDir.Round();
			switch (direction)
			{
				case var v when v == Vector2.Right: animationState = "walk_right"; break;
				case var v when v == Vector2.Left: animationState = "walk_left"; break;
				case var v when v == Vector2.Up: animationState = "walk_up"; break;
				case var v when v == Vector2.Down: animationState = "walk_down"; break;
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

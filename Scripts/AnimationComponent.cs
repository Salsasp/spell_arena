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
			switch (GetDirectionIndex(direction))
			{
				case 0: animationState = "walk_right"; break;
				case 1: animationState = "walk_down_right"; break;
				case 2: animationState = "walk_down"; break;
				case 3: animationState = "walk_down_left"; break;
				case 4: animationState = "walk_left"; break;
				case 5: animationState = "walk_up_left"; break;
				case 6: animationState = "walk_up"; break;
				case 7: animationState = "walk_up_right"; break;
			}
			animatedSprite.Play(animationState);
		}
		
	}

	private int GetDirectionIndex(Vector2 vec)
    {
        if (vec == Vector2.Zero)
            return 0;

        float angle = vec.Angle();
        int index = Mathf.RoundToInt(angle / (Mathf.Pi / 4.0f));
        index = ((index % 8) + 8) % 8;
        return index;
    }
}

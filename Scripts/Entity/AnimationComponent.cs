using Godot;
using System;

public partial class AnimationComponent : Node2D
{
	[Export] public AnimatedSprite2D animatedSprite;
	[Export] public AnimationPlayer animationPlayer;
	[Export] public bool directionalSprites;
	[Export] public bool isPlayer;
	[Export] public bool isEnemy;
	[Signal] public delegate void FallAnimationFinishedEventHandler();
	public bool IsPlayingFall { get; private set; }
	private string animationState = "idle";
	private Node2D parent;

	public override void _Ready()
	{
		parent = GetParent<Node2D>();
		animationPlayer.AnimationFinished += OnAnimationFinished;
	}

	public void Update(MovementComponent movementComponent = null)
	{
		// Ignore animated movement if npc doesn't move / non player animation
		if (directionalSprites && isPlayer && movementComponent != null)
		{
			Vector2 direction = movementComponent.mouseDir.Round();
			switch (Util.GetDirectionIndex8D(direction))
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

		if (directionalSprites && isEnemy && movementComponent != null)
		{
			Vector2 dir = movementComponent.direction;
			if (dir.X < 0)
			{
				animationState = "walk_left";
			}
			else if (dir.X >= 0)
			{
				animationState = "walk_right";
			}
			else
			{
				animationState = "idle";
			}

			animatedSprite.Play(animationState);
		}
	}

	private void OnAnimationFinished(StringName animName)
	{
		if (animName != "fall")
			return;
		IsPlayingFall = false;
		EmitSignal(SignalName.FallAnimationFinished);
	}

	public bool TryPlayFallAnimation(Pitfall pitfall)
	{
		if (IsPlayingFall || !animationPlayer.HasAnimation("fall"))
			return false;

		IsPlayingFall = true;
		animationPlayer.Play("fall");
		return true;
	}
}

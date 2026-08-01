using Godot;
using System;

public partial class Pitfall : Node2D
{
	[Export] public Sprite2D PitfallSprite;
	[Export] public Area2D FallArea;
	[Export] public Area2D SwipeArea;
	[Export] public Node2D SkeletalHand;

	public override void _Ready()
	{
		FallArea.BodyEntered += OnFallAreaEntered;
		SwipeArea.BodyEntered += OnSwipeAreaEntered;
	}

	private void OnSwipeAreaEntered(Node2D body)
	{
		
	}

	private void OnFallAreaEntered(Node2D body)
	{
		
	}
}

using Godot;
using System;

public partial class Wand : Node2D
{
	[Export] public Marker2D EndOfWand;
	[Export] public Sprite2D WandSprite;

	public override void _Ready()
	{
	}

	
	public override void _Process(double delta)
	{
	}

	public void RotateWand(InputComponent inputComponent)
	{
		Vector2 dir = EndOfWand.GlobalPosition.DirectionTo(inputComponent.MousePos).Normalized();
		Rotation = dir.Angle();
	}

	public Vector2 GetEndOfWandPos()
	{
		return EndOfWand.GlobalPosition;
	}
}

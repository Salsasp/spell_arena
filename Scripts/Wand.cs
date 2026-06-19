using Godot;
using System;
using System.ComponentModel;

public partial class Wand : Node2D
{
	[Export] public Marker2D EndOfWand;
	[Export] public Sprite2D WandSprite;
	[Export] public Node2D WandPivot;

	public override void _Ready()
	{
	}

	
	public override void _Process(double delta)
	{
	}

	public void RotateWand(InputComponent inputComponent)
	{
		Vector2 dir = EndOfWand.GlobalPosition.DirectionTo(inputComponent.MousePos).Normalized();
		WandPivot.Rotation = dir.Angle();
	}

	public Vector2 GetEndOfWandPos()
	{
		return EndOfWand.GlobalPosition;
	}
}

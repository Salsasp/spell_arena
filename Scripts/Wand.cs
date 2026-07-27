using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Wand : Node2D
{
	[Export] public Marker2D EndOfWand;
	[Export] public Sprite2D WandSprite;
	[Export] public Node2D WandPivot;
	public Dictionary<int, Marker2D> IndexToDirectionSnapDict;

	public override void _Ready()
	{
	}

	public void SnapPivotToDirection(MovementComponent movementComponent, Node2D wandSnapPoints)
	{
		Vector2 direction = movementComponent.mouseDir.Round();
			switch (Util.GetDirectionIndex8D(direction))
			{
				case 0: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Right").GlobalPosition; break;
				case 1: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("DownRight").GlobalPosition; break;
				case 2: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Down").GlobalPosition; break;
				case 3: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("DownLeft").GlobalPosition; break;
				case 4: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Left").GlobalPosition; break;
				case 5: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("UpLeft").GlobalPosition; break;
				case 6: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Up").GlobalPosition; break;
				case 7: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("UpRight").GlobalPosition; break;
			}
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

using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class Wand : Node2D
{
	[Export] public Marker2D EndOfWand;
	[Export] public Sprite2D WandSprite;
	[Export] public Node2D WandPivot;
	public WandInventory wandInventory = new();
	public ModifierData InnateMod;

	public override void _Ready()
	{
	}

	public void SnapPivotToDirection(MovementComponent movementComponent, Node2D wandSnapPoints)
	{
		Vector2 direction = movementComponent.mouseDir.Round();
			switch (Util.GetDirectionIndex8D(direction))
			{
				case 0: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Right").GlobalPosition; ZIndex = 1; break;
				case 1: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("DownRight").GlobalPosition; ZIndex = 1; break;
				case 2: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Down").GlobalPosition; ZIndex = 1; break;
				case 3: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("DownLeft").GlobalPosition; ZIndex = 1; break;
				case 4: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Left").GlobalPosition; ZIndex = 1; break;
				case 5: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("UpLeft").GlobalPosition; ZIndex = 1; break;
				case 6: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("Up").GlobalPosition; ZIndex = 0; break;
				case 7: WandPivot.GlobalPosition = wandSnapPoints.GetNode<Marker2D>("UpRight").GlobalPosition; ZIndex = 0; break;
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

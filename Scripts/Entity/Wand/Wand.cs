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
	public virtual ModifierAffinity Affinity => ModifierAffinity.Neutral;

	public bool CanAccept(ModifierData modifier)
	{
		if (modifier == null)
		{
			return true;
		}

		if (modifier.IsRunScoped)
		{
			return false;
		}

		if (modifier.affinity == ModifierAffinity.Neutral)
		{
			return true;
		}

		return modifier.affinity == Affinity;
	}

	public override void _EnterTree()
	{
		SortLayers.Apply(this, SortLayers.Wands);
	}

	public override void _Ready()
	{
	}

	public void SnapPivotToDirection(MovementComponent movementComponent, Node2D wandSnapPoints)
	{
		int directionIndex = Util.GetDirectionIndex8D(movementComponent.mouseDir.Round());
		switch (directionIndex)
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

		bool behindEntities = directionIndex is 6 or 7;
		SortLayers.Apply(this, behindEntities ? SortLayers.Projectiles : SortLayers.Wands);
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

	public List<ModifierData> GetCombatModifiers()
	{
		List<ModifierData> modifiers = [];
		ModifierData innate = InnateMod ?? wandInventory.GetInnateMod();
		if (innate != null)
		{
			modifiers.Add(innate);
		}

		modifiers.AddRange(wandInventory.GetMods());
		return modifiers;
	}
}

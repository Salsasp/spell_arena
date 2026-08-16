using Godot;
using System.Collections.Generic;

public partial class ModifyWandUI : Control
{
	[Signal] public delegate void ContinuePressedEventHandler();

	[Export] public GridContainer ArcaneGrid;
	[Export] public GridContainer LightningGrid;
	[Export] public GridContainer FireGrid;
	[Export] public GridContainer IceGrid;
	[Export] public GridContainer InventoryGrid;
	[Export] public Button ContinueButton;

	private Player player;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Visible = false;
		if (ContinueButton != null)
		{
			ContinueButton.Pressed += OnContinuePressed;
		}
	}

	public void ShowCustomize(Player targetPlayer)
	{
		player = targetPlayer;
		BindAll();
		Refresh();
		Visible = true;
		MoveToFront();
	}

	public void HideCustomize()
	{
		Visible = false;
	}

	public void Refresh()
	{
		if (ArcaneGrid == null)
		{
			return;
		}

		foreach (InventoryModifierSlot slot in GetAllSlots())
		{
			slot.Refresh();
		}
	}

	public bool CanMoveModifier(InventoryModifierSlot source, InventoryModifierSlot destination)
	{
		if (source == null || destination == null || source == destination)
		{
			return false;
		}

		if (source.AssignedModifier == null || source.Inventory == null || destination.Inventory == null)
		{
			return false;
		}

		if (!CanAccept(destination, source.AssignedModifier))
		{
			return false;
		}

		ModifierData destinationModifier = destination.AssignedModifier;
		if (destinationModifier != null && !CanAccept(source, destinationModifier))
		{
			return false;
		}

		return true;
	}

	public void MoveModifier(InventoryModifierSlot source, InventoryModifierSlot destination)
	{
		if (!CanMoveModifier(source, destination))
		{
			return;
		}

		ModifierData moving = source.Inventory.GetSlot(source.SlotIndex);
		ModifierData replaced = destination.Inventory.GetSlot(destination.SlotIndex);
		destination.Inventory.SetSlot(destination.SlotIndex, moving);
		source.Inventory.SetSlot(source.SlotIndex, replaced);
		source.Refresh();
		destination.Refresh();
	}

	private void BindAll()
	{
		if (player == null)
		{
			return;
		}

		BindGrid(ArcaneGrid, player.ArcaneWand.wandInventory.Slots, player.ArcaneWand);
		BindGrid(LightningGrid, player.LightningWand.wandInventory.Slots, player.LightningWand);
		BindGrid(FireGrid, player.FireWand.wandInventory.Slots, player.FireWand);
		BindGrid(IceGrid, player.IceWand.wandInventory.Slots, player.IceWand);
		BindGrid(InventoryGrid, player.ModifierPool, null);
	}

	private void BindGrid(GridContainer grid, ModifierInventory inventory, Wand hostWand)
	{
		int index = 0;
		foreach (Node child in grid.GetChildren())
		{
			if (child is not InventoryModifierSlot slot)
			{
				continue;
			}

			slot.Bind(this, inventory, index, hostWand);
			index++;
		}
	}

	private static bool CanAccept(InventoryModifierSlot slot, ModifierData modifier)
	{
		if (slot.HostWand == null)
		{
			return true;
		}

		return slot.HostWand.CanAccept(modifier);
	}

	private IEnumerable<InventoryModifierSlot> GetAllSlots()
	{
		foreach (GridContainer grid in new[] { ArcaneGrid, LightningGrid, FireGrid, IceGrid, InventoryGrid })
		{
			if (grid == null)
			{
				continue;
			}

			foreach (Node child in grid.GetChildren())
			{
				if (child is InventoryModifierSlot slot)
				{
					yield return slot;
				}
			}
		}
	}

	private void OnContinuePressed()
	{
		EmitSignal(SignalName.ContinuePressed);
	}
}
